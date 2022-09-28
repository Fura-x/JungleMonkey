using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jump : MonoBehaviour
{
    [Header("Air control in Move script")]
    [Space]
    public bool canMove = true;

    Rigidbody rb = null;
    [SerializeField] PhysicMaterial noFriction = null;
    Collider playerPhysics;

    [SerializeField] float jumpForce = 6f;
    [SerializeField] float bumperForce = 12f;
    [SerializeField] float grabJumpForce = 7.5f;
    [SerializeField] float wallJumpForce = 7.5f;
    [SerializeField] float swayHighJumpForce = 7.5f;
    [SerializeField] float slowMultiplier = 2f;

    Vector3 offsetRayLeft;
    Vector3 offsetRayRight;
    float rayLength = 0.1f;
    Collider col;

    RaycastHit infoLeft = new RaycastHit();
    RaycastHit infoRight = new RaycastHit();

    bool isGrab = false;
    public bool isGrabbingBranch = false;
    [Header("Enable sway high jump")]
    public bool highJumpEnable = false;

    public LayerMask mask;


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponentInChildren<Collider>();
        playerPhysics = GetComponentInChildren<CapsuleCollider>();

        offsetRayLeft = Vector3.right * col.bounds.extents.x / 2;
        offsetRayRight = Vector3.right * -col.bounds.extents.x / 2;
        rayLength += col.bounds.extents.y;
    }

    public void SetGeneralGrabState(bool isGrabOrSway)
    {
        isGrab = isGrabOrSway;
    }

    bool castLeftValid()
    {
        return Physics.Raycast(transform.position + offsetRayLeft, -Vector3.up, 
                               out infoLeft, rayLength, mask) 
                               && !infoLeft.transform.CompareTag("Slope");
    }
    bool castRightValid()
    {
        return Physics.Raycast(transform.position + offsetRayRight, -Vector3.up, 
                               out infoRight, rayLength, mask) 
                               && !infoRight.transform.CompareTag("Slope");
    }
    // Start is called before the first frame update
    public void JumpPerform(float move)
    {
        if (isGrab)
        {
            float wallJumpFactor = Mathf.Abs(move) <= 0.25f ? 0f : Mathf.Sign(move) * wallJumpForce;

            float highJumpFactor = 0f;
            if (isGrabbingBranch && highJumpEnable) highJumpFactor = swayHighJumpForce * (rb.velocity.y >= 10f ? 3f : rb.velocity.y * 0.3f);
            else highJumpFactor = grabJumpForce;

            rb.velocity = new Vector3(rb.velocity.x, highJumpFactor, 0) + Vector3.right * wallJumpFactor;
        }
        else if ( castLeftValid() || castRightValid() )
        {
            rb.velocity = new Vector3(rb.velocity.x, jumpForce, 0);
            GetComponent<PlayerSound>().Play("Jump");
        }
    }

    public void SlowJump()
    {
        jumpForce /= slowMultiplier;
    }

    public void UnslowJump()
    {
        jumpForce *= slowMultiplier;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Bumper"))
        {
            float bump = collision.collider.GetComponent<Bumper>().bumpForce;
            rb.velocity = new Vector3(rb.velocity.x, (bump == 0f ? bumperForce : bump), 0);
        }

        if (castLeftValid() || castRightValid())
        {
            canMove = true;
            playerPhysics.material = null;
        }
    }
    private void OnCollisionExit(Collision collision)
    {

        if (!castLeftValid() && !castRightValid())
        {
            canMove = false;
            playerPhysics.material = noFriction;
        }
    }
}
