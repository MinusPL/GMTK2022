using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private Animator animator;
    private CharacterController cc;

    private float smoothAngle;
    public float rotateSmoothTime = 0.1f;

    private float targetAngle = 90f;

    //Visual Range
    [SerializeField]
    private float VisualRange = 10f;
    [SerializeField]
    private float AttackRange = 2f;


    private GameObject player;

    private float ySpeed = 0;

    private bool isGrounded = true;

    //Attack stuff
    [SerializeField]
    private float AttackCooldown = 2f;
    private float AttackTimer = 0f;
    [SerializeField]
    private float AttackDamage = 20f;
    private bool canAttack = true;

    [SerializeField]
    private float attackDamageDelay = 0.5f;
    private float attackDamageDelayTimer = 0f;
    [SerializeField]
    private AttackVolume attackColider;

    private bool dead = false;
    public bool aiEnabled = false;

    public float damageMultiplier = 1f;


    [SerializeField]
    private AudioSource footsteps;
    [SerializeField]
    private AudioSource swing_sword;

    // Start is called before the first frame update
    void Start()
    {
        transform.rotation = Quaternion.Euler(0f, 90f, 0f);
        player = GameObject.FindGameObjectWithTag("Player");

        animator = GetComponent<Animator>();
        cc = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!dead && aiEnabled)
        {
            Vector3 dir = Vector3.zero;

            float dist = Vector3.Distance(transform.position, player.transform.position);
            if (dist <= VisualRange && dist > 1.5f)
            {
                dir = (player.transform.position - transform.position).normalized;
            }

            if (dist <= AttackRange && canAttack)
            {
                canAttack = false;
                animator.SetTrigger("Attack");
                AttackTimer = AttackCooldown;
                attackDamageDelayTimer = attackDamageDelay;
            }

            if (dir.magnitude > 0.01f)
            {
                animator.SetBool("isMoving", true);
                if (GameManager.Instance.playerInputEnabled)
                    targetAngle = dir.x > 0 ? 90f : -90f;
                if (!footsteps.isPlaying) footsteps.Play();
            }
            else
            {
                animator.SetBool("isMoving", false);
                if (footsteps.isPlaying) footsteps.Stop();
            }

            ySpeed += Physics.gravity.y * Time.deltaTime;

            if (transform.eulerAngles.y != targetAngle)
            {
                float smoothTarget = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref smoothAngle, rotateSmoothTime);
                transform.rotation = Quaternion.Euler(0f, smoothTarget, 0f);
            }

            if (AttackTimer > 0f)
            {
                AttackTimer -= Time.deltaTime;
                if (AttackTimer <= 0f)
                {
                    AttackTimer = 0f;
                    canAttack = true;
                }
            }

            if (attackDamageDelayTimer > 0f)
            {
                attackDamageDelayTimer -= Time.deltaTime;
                if (attackDamageDelayTimer <= 0f)
                {
                    attackDamageDelayTimer = 0f;
                    if (attackColider.playerInRange)
                    {
                        player.GetComponent<LivingEntity>().Damage(AttackDamage*damageMultiplier);
                        swing_sword.Play();
                    }
                }
            }
        }
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
        if (Mathf.Abs(transform.position.z - 0f) > 0.1f)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, 0f);
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, VisualRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, AttackRange);
    }

    public void Death()
    {
        dead = true;
        cc.height = 0.2f;
        GameManager.Instance.OnEnemyDie(this);
        //cc.center = new Vector3(cc.center.x, 0.1f, cc.center.z);
    }
}
