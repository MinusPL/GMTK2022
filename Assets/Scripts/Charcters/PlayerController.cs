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


    private float targetAngle = 0f;

    //For last part of level
    //Also decide if character is jumping out or walking away.
    private List<GameObject> waypoints;
    private int waypointIndex = 0;

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

        Debug.DrawRay(transform.position, dir, Color.red);

        if (dir.magnitude > 0.01f)
        {
            animator.SetBool("isMoving", true);
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

            if (Time.time - jumpButtonPressedTime <= jumpButtonGracePeriod)
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

    public void SetWaypoints(List<GameObject> _waypoints)
    {
        waypoints = _waypoints;

    }
}
