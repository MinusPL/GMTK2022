using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    private Vector2 direction;
    private Animator animator;
    private CharacterController cc;
    private float smoothAngle;
    public float rotateSmoothTime = 0.1f;
    public GameObject CameraPlayer;

    [SerializeField]
    private float ySpeed = 0;

    private float lastGroundedTime = 0;

    private bool isJumping = false;
    private bool isGrounded = true;

    private float originalStepOffset;

    [SerializeField]
    private float jumpButtonGracePeriod;

    [SerializeField]
    private float jumpSpeed = 10.0f;

    private float jumpButtonPressedTime = 0.0f;


    private Vector3 animatorDirection = Vector3.zero;

    [SerializeField]
    private float jumpHorizontalSpeedRun = 5.0f;

    [SerializeField]
    private float characterHeight = 1.62f;
    [SerializeField]
    private float characterJumpingHeight = 1.0f;


    private float targetAngle = 90f;

    //For last part of level
    //Also decide if character is jumping out or walking away.
    private List<GameObject> waypoints;
    private int waypointIndex = 0;


    private bool isArmed = false;
    [SerializeField]
    private GameObject swordBeltObject;
    [SerializeField]
    private GameObject swordHandObject;
    [SerializeField]
    private float WeaponToggleBlockTime = 0.5f;
    private float WeaponToggleTimer = 0f;
    private bool canToggleWeapon = true;
    [SerializeField]
    private float attackBlockTime = 0.5f;
    private float attackBlockTimer = 0f;
    private bool canAttack = true;

    [SerializeField]
    private float NormalAttackDamage = 20f;
    [SerializeField]
    private float NormalAttackDamageDelay = 1f;
    private float NADamageDelayTimer = 0f;
    [SerializeField]
    private float PowerAttackDamage = 40f;
    [SerializeField]
    private float PowerAttackDamageDelay = 2f;
    private float PADamageDelayTimer = 0f;


    [SerializeField]
    private AttackVolumePlayer avp;


    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        cc = GetComponent<CharacterController>();
        originalStepOffset = cc.stepOffset;
        //uiController = GameObject.FindGameObjectWithTag("UIControl").GetComponent<UIController>();
        jumpButtonPressedTime = -Mathf.Infinity;
        lastGroundedTime = Mathf.Infinity;
        waypoints = new List<GameObject>();
    }

    private void Update()
    {
        Vector3 dir = Vector3.zero;
        if (GameManager.Instance.playerInputEnabled)
        {
            dir = new Vector3(direction.x, 0.0f, direction.y).normalized;
        }
        else
        {
            if (waypoints.Count > 0)
            {
                dir = (waypoints[waypointIndex].transform.position - transform.position).normalized;
                if (Vector3.Distance(transform.position, waypoints[waypointIndex].transform.position) <= 0.5f)
                    waypointIndex++;
                if (waypointIndex >= waypoints.Count)
                {
                    waypoints.Clear();
                }
            }
        }

        if (dir.magnitude > 0.01f)
        {
            animator.SetBool("isMoving", true);
            animator.SetBool("isArmed", isArmed);
            //animator.SetBool("isSprint", false);
            //animator.SetFloat("dirMagnitude", 0f);
            if (GameManager.Instance.playerInputEnabled)
                targetAngle = dir.x > 0 ? 90f : -90f;
            else
                targetAngle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
        }
        else
        {
            animator.SetBool("isMoving", false);
            animator.SetBool("isArmed", isArmed);
            //animator.SetBool("isSprint", false);
        }

        if (transform.eulerAngles.y != targetAngle)
        { 
            float smoothTarget = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref smoothAngle, rotateSmoothTime);
            transform.rotation = Quaternion.Euler(0f, smoothTarget, 0f);
        }

        ySpeed += Physics.gravity.y * Time.deltaTime;

        if (cc.isGrounded)
        {
            lastGroundedTime = Time.time;
        }

        if (Time.time - lastGroundedTime <= jumpButtonGracePeriod)
        {
            cc.stepOffset = originalStepOffset;
            if (cc.isGrounded)
                ySpeed = -0.5f;
            else
                ySpeed += Physics.gravity.y * Time.deltaTime;
            isJumping = false;
            isGrounded = true;

            animator.SetBool("isGrounded", true);
            animator.SetBool("isJumping", false);
            animator.SetBool("isFalling", false);
            cc.height = characterHeight;
            animatorDirection = Vector3.zero;

            if (Time.time - jumpButtonPressedTime <= jumpButtonGracePeriod && !isArmed)
            {
                ySpeed = jumpSpeed;
                animator.SetBool("isJumping", true);
                isJumping = true;
                cc.height = characterJumpingHeight;
                jumpButtonPressedTime = 0f;
                lastGroundedTime = 0f;
                if (direction.magnitude > 0.001f)
                    animatorDirection = transform.forward * jumpHorizontalSpeedRun;
            }

        }
        else
        {
            cc.stepOffset = 0f;
            animator.SetBool("isGrounded", false);
            isGrounded = false;

            if ((isJumping && ySpeed < 0) || ySpeed < -2f)
            {
                animator.SetBool("isFalling", true);
                cc.height = characterJumpingHeight;
                if (animatorDirection == Vector3.zero)
                {
                    if (direction.magnitude > 0.001f)
                        animatorDirection = transform.forward * jumpHorizontalSpeedRun;
                }
            }
        }

        if ((cc.collisionFlags & CollisionFlags.Above) != 0 && ySpeed > 0f)
            ySpeed = 0f;

        if ((cc.collisionFlags & CollisionFlags.Above) != 0)
        {
            lastGroundedTime = Time.deltaTime;
            jumpButtonPressedTime = 0f;
        }

        if (!isGrounded)
        {
            Vector3 velocity = animatorDirection;

            velocity.y = ySpeed;
            cc.Move(velocity * Time.deltaTime);
        }

        if (isArmed && animator.GetCurrentAnimatorStateInfo(0).IsTag("SwordDrawn2"))
        {
            swordBeltObject.SetActive(false);
            swordHandObject.SetActive(true);
        }

        if (!isArmed && animator.GetCurrentAnimatorStateInfo(0).IsTag("SwordSheathed2"))
        {
            swordBeltObject.SetActive(true);
            swordHandObject.SetActive(false);
        }

        if (WeaponToggleTimer > 0f)
        {
            WeaponToggleTimer -= Time.deltaTime;
            if (WeaponToggleTimer <= 0f)
            {
                WeaponToggleTimer = 0f;
                canToggleWeapon = true;
            }
        }

        if (attackBlockTimer > 0f)
        {
            attackBlockTimer -= Time.deltaTime;
            if (attackBlockTimer <= 0f)
            {
                attackBlockTimer = 0f;
                canAttack = true;
            }
        }

        if(NADamageDelayTimer > 0f)
        {
            NADamageDelayTimer -= Time.deltaTime;
            if(NADamageDelayTimer <= 0f)
            {
                NADamageDelayTimer = 0f;
                DealDamage(NormalAttackDamage);
            }
        }

        if (PADamageDelayTimer > 0f)
        {
            PADamageDelayTimer -= Time.deltaTime;
            if (PADamageDelayTimer <= 0f)
            {
                PADamageDelayTimer = 0f;
                DealDamage(PowerAttackDamage);
            }
        }

        //if (canAttack && !(animator.GetCurrentAnimatorStateInfo(0).IsName("Normal Attack") || animator.GetCurrentAnimatorStateInfo(0).IsName("Power Attack")))
        //{
        //    currentWeapon.isAttack = false;
        //    currentWeapon.ClearHits();
        //}
    }

    public void OnAnimatorMove()
    {
        if (isGrounded)
        {
            Vector3 velocity = animator.deltaPosition;
            velocity = AdjustVelocityToSlope(velocity);
            velocity.y += ySpeed * Time.deltaTime;
            cc.Move(velocity);
        }
        if (GameManager.Instance.playerInputEnabled && Mathf.Abs(transform.position.z - 0f) > 0.1f)
        {
            transform.position =  new Vector3(transform.position.x, transform.position.y, 0f);
        }

    }

    private Vector3 AdjustVelocityToSlope(Vector3 velocity)
    {
        Ray ray = new Ray(transform.position, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, 0.2f))
        {
            Quaternion slopeRotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
            Vector3 newVelocity = slopeRotation * velocity;
            if (newVelocity.y < 0f) return newVelocity;
        }
        return velocity;
    }


    public void OnMove(InputValue value)
    {
        direction = new Vector2(value.Get<Vector2>().x , 0f);
    }

    public void OnJump(InputValue value)
    {
        if (value.isPressed)
        {
            jumpButtonPressedTime = Time.time;
        }
    }

    public void OnFire(InputValue value)
    {
        if (!GameManager.Instance.playerInputEnabled)
            return;

        if (!isArmed)
        {
            if (canToggleWeapon && (animator.GetCurrentAnimatorStateInfo(0).IsName("Move") ||
                                    animator.GetCurrentAnimatorStateInfo(0).IsName("Idle") ||
                                    animator.GetCurrentAnimatorStateInfo(0).IsName("Idle Sword") ||
                                    animator.GetCurrentAnimatorStateInfo(0).IsName("Sword Run")))
            {
                isArmed = true;
                canToggleWeapon = false;
                WeaponToggleTimer = WeaponToggleBlockTime;
            }
            //Return since we just drew the weapon, we're not going to attack
            return;
        }
        else
        {
            //Figure out how to attack now
            if(canAttack && (animator.GetCurrentAnimatorStateInfo(0).IsName("Idle Sword") || animator.GetCurrentAnimatorStateInfo(0).IsName("Sword Run")))
            {
                animator.SetTrigger("Attack");
                canAttack = false;
                attackBlockTimer = attackBlockTime;
                NADamageDelayTimer = NormalAttackDamageDelay;
            }
        }
    }

    public void OnPowerAttack(InputValue value)
    {
        if (!GameManager.Instance.playerInputEnabled)
            return;

        if (!isArmed)
        {
            if (canToggleWeapon && (animator.GetCurrentAnimatorStateInfo(0).IsName("Move") ||
                                    animator.GetCurrentAnimatorStateInfo(0).IsName("Idle") ||
                                    animator.GetCurrentAnimatorStateInfo(0).IsName("Idle Sword") ||
                                    animator.GetCurrentAnimatorStateInfo(0).IsName("Sword Run")))
            {
                isArmed = true;
                canToggleWeapon = false;
                WeaponToggleTimer = WeaponToggleBlockTime;
            }
            //Return since we just drew the weapon, we're not going to attack
            return;
        }
        else
        {
            //Figure out how to attack now
            if (canAttack && (animator.GetCurrentAnimatorStateInfo(0).IsName("Idle Sword") || animator.GetCurrentAnimatorStateInfo(0).IsName("Sword Run")))
            {
                animator.SetTrigger("PowerAttack");
                canAttack = false;
                attackBlockTimer = attackBlockTime;
                PADamageDelayTimer = PowerAttackDamageDelay;
            }
        }
    }

    public void OnSheatheWeapon(InputValue value)
    {
        if (!GameManager.Instance.playerInputEnabled)
            return;

        if (canToggleWeapon && (animator.GetCurrentAnimatorStateInfo(0).IsName("Move") ||
                                animator.GetCurrentAnimatorStateInfo(0).IsName("Idle") ||
                                animator.GetCurrentAnimatorStateInfo(0).IsName("Idle Sword") ||
                                animator.GetCurrentAnimatorStateInfo(0).IsName("Sword Run")))
        {
            isArmed = !isArmed;
            canToggleWeapon = false;
            WeaponToggleTimer = WeaponToggleBlockTime;
        }
    }

    private void DealDamage(float dmg)
    {
        List<Enemy> enemies = new List<Enemy>(avp.enemiesInRange);
        foreach (var enemy in enemies)
        {
            enemy.Damage(dmg);
        }
    }

    public void CheckDeadEnemy(Enemy enemy)
    {
        
        if (avp.enemiesInRange.Contains(enemy)) avp.enemiesInRange.Remove(enemy);
    }

    public void SetWaypoints(List<GameObject> _waypoints)
    {
        waypoints = _waypoints;

    }
}
