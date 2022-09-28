
using UnityEngine;

[RequireComponent (typeof(Sway))]
public class Tail : MonoBehaviour
{
    // Use to throw tail only one frame
    bool isThrew = false;
    // Use to signal that player is grab to a wall/tree branch
    bool prevIsGrab = false;
    // use when Player grabbed a platform from below
    bool prevIsSway = false;

    bool fastThrow = false;

    //Script links
    Jump jump = null;
    Grab grab = null;
    Sway sway = null;
    Crouch crouch = null;
    PlayerControl controls = null;
    PlayerSound sound = null;

    Vector2 angle = Vector3.up;

    Move playerMove;
    Transform tail;
    [SerializeField] GameObject tailBase = null;

    RaycastHit tailHitInfo = new RaycastHit();
    Vector3 lastHitPoint;

    [SerializeField] float maxLength = 3f;
    [SerializeField] float hitRange = 0.2f;

    [SerializeField] [Range(0f, 60f)] float swayAngleLimit = 45f;
    [SerializeField] [Range(0f, 60f)] float grabAngleLimit = 45f;
    [SerializeField] [Range(0, .9f)] float stickDeadZone = .3f;

    int orientation = 1;

    Vector3 baseScale = Vector3.up;
    Vector3 maxScale = Vector3.up;
    Vector3 anchorPosition = Vector3.zero;

    LayerMask mask;

    public bool CanRotate()
    {
        return tail.localScale.y == 1f;
    }

    public void SetOrientation(int positiveOrNegative)
    {
        orientation = positiveOrNegative;
    }

    private void Start()
    {
        tail = tailBase.GetComponentInChildren<Transform>();
        playerMove = GetComponentInParent<Move>();
        sway = GetComponent<Sway>();
        grab = GetComponent<Grab>();
        crouch = GetComponent<Crouch>();
        jump = GetComponent<Jump>();
        sound = GetComponent<PlayerSound>();

        baseScale = tail.transform.localScale;
        maxScale = new Vector3(baseScale.x, maxLength, baseScale.z);
        sway.maxDistanceFromHit = maxLength * 2f;
        mask = jump.mask;

        // CONTROLS

        controls = playerMove.controls;
        // Right Joystick triggered
        controls.Player.Look.performed += ctx =>
        {
            if (!crouch.isEnabled && !prevIsSway)
            {
                if (ctx.ReadValue<Vector2>().magnitude > stickDeadZone)
                {
                    angle = ctx.ReadValue<Vector2>();
                }
                else ResetTail();
            }
        };
        // Right Joystick released
        controls.Player.Look.canceled += ctx =>
        {
            if (!prevIsSway) ResetTail();
        };
        // Fast tail 
        controls.Player.Jump.performed += ctx =>
        {
            if(!jump.canMove) FastThrow();
        };
        controls.Player.Jump.canceled += ctx =>
        {
            if (!prevIsSway) ResetTail();
        };

    }

    void ResetTail()
    {
        tail.transform.localScale = baseScale;
        angle = Vector2.up;
        isThrew = false;
    }

    public void FastThrow()
    {
        if (!isThrew && !prevIsGrab) fastThrow = true;
    }

    void FastThrowPerform()
    {
        angle = orientation == 1 ? new Vector2(1f, 1f) : new Vector2(-1f, 1f);
        tailBase.transform.up = angle;
        TailGrowth(angle);
        fastThrow = false;

        sound.Play("Tail");
    }

    void TailGrowth(Vector2 direction)
    {
        if (!direction.Equals(Vector2.up) && !prevIsSway)
        {
            direction.Normalize();

            Vector3 pos = tailBase.transform.position;
            Vector3 point1 = new Vector3(pos.x - hitRange, pos.y, pos.z);
            Vector3 point2 = new Vector3(pos.x + hitRange, pos.y, pos.z);
            // Adapt size according to raycast collision or max length
            if (Physics.Raycast(pos, direction, out tailHitInfo, maxLength * 2f, mask))
            {
                tail.localScale = new Vector3(baseScale.x, tailHitInfo.distance / 2f, baseScale.z);
            }
            else if (Physics.SphereCast(pos, hitRange, direction, out tailHitInfo, maxLength * 2f, mask))
            //else if (Physics.CapsuleCast(point1, point2, hitRange, angle, out tailHitInfo, maxLength * 2f))
            {
                tail.localScale = new Vector3(baseScale.x, tailHitInfo.distance / 2f, baseScale.z);
            }
            else
            {
                tail.localScale = maxScale;
            }

            isThrew = true;
            sound.Play("Tail");
        }
    }

    private float GetAngle(Vector3 v1, Vector3 v2)
    {
        float angle = Mathf.Atan2(v1.y, v1.x) - Mathf.Atan2(v2.y, v2.x);
        return Mathf.Rad2Deg * angle;
    }

    private void ReadTailHitInfo()
    {
        lastHitPoint = tailHitInfo.point;

        if (tailHitInfo.collider.CompareTag("Floor"))
        {
            Vector3 normal = tailHitInfo.normal;
            float swayAngle = GetAngle(Vector3.down, normal);
            float grabAngleRight = GetAngle(Vector3.right, normal);
            float grabAngleLeft = GetAngle(Vector3.left, normal);
            grabAngleLeft -= (grabAngleLeft > 180f ? 360f : 0f);

            // Wall Collision => Grab as a grappin
            if ((grabAngleRight <= grabAngleLimit && grabAngleRight >= -grabAngleLimit)
                || (grabAngleLeft <= grabAngleLimit && grabAngleLeft >= -grabAngleLimit))
            {
                prevIsGrab = true;
                grab.Enable(tailHitInfo.point);
            }
            // Ceiling collision => balance monkey
            else if (swayAngle <= swayAngleLimit && swayAngle >= -swayAngleLimit)
            {
                if (tailHitInfo.collider.gameObject.GetComponent<Rigidbody>() != null)
                {
                    anchorPosition = tailHitInfo.point;
                    sway.Enable(anchorPosition, tailHitInfo.collider.gameObject.GetComponent<Rigidbody>());
                    if (grab.isEnabled) grab.Disable(); 
                }
            }

            jump.isGrabbingBranch = false;
        }
        else if (tailHitInfo.collider.CompareTag("Branch"))
        {
            if (tailHitInfo.collider.gameObject.GetComponent<Rigidbody>() != null)
            {
                anchorPosition = tailHitInfo.transform.position;
                sway.Enable(anchorPosition, tailHitInfo.collider.gameObject.GetComponent<Rigidbody>());
                if (grab.isEnabled) grab.Disable();

                jump.isGrabbingBranch = true;
            }
        }

        switch (tailHitInfo.collider.tag)
        {
            case "WeakPoint":
                if (tailHitInfo.rigidbody.gameObject.GetComponent<HippoHit>() != null)
                    tailHitInfo.rigidbody.gameObject.GetComponent<HippoHit>().KnockOut();
                else if (tailHitInfo.rigidbody.gameObject.GetComponent<RhinoBehavior>() != null)
                    tailHitInfo.rigidbody.gameObject.GetComponent<RhinoBehavior>().ForceCharge();
                else if (tailHitInfo.rigidbody.gameObject.GetComponent<ScratchPanthereBehavior>() != null)
                    tailHitInfo.rigidbody.gameObject.GetComponent<ScratchPanthereBehavior>().Grabbed();
                break;

            case "Log":
                tailHitInfo.collider.GetComponent<LogScript>().Fall();
                break;
            case "Cobra":
                tailHitInfo.collider.GetComponentInParent<CobraBehaviour>().Stun();
                break;
            case "LittleBird":
                tailHitInfo.collider.GetComponentInParent<LittleBirdBehavior>().RunAway();
                break;
            case "BigBird":
                tailHitInfo.collider.GetComponentInParent<BigBirdBehavior>().Stun();
                break;
            default:
                break;
        }
    }

    private void FixedUpdate()
    {
        tailBase.transform.up = angle;

        // TAIL SIZE MANAGE
        if (fastThrow) FastThrowPerform();
        else if (!isThrew) TailGrowth(angle);
        else if (!prevIsSway)
        {
            tail.localScale = Vector3.Lerp(tail.localScale, baseScale, 0.2f);
            tailHitInfo = new RaycastHit();

            prevIsGrab = false;
        }

        // TAIL ROTATION MANAGE
        if (prevIsSway)
        {
            angle = (anchorPosition - tailBase.transform.position).normalized;
            tail.localScale = new Vector3(baseScale.x, Vector3.Distance(lastHitPoint, tail.position) / 2f, baseScale.z);

            if (prevIsSway != sway.isEnabled)
                ResetTail();
        }
        //if (crouch.isEnabled) 
        //    tail.up = orientation < 0 ? new Vector3(1f, 1f, 0) : new Vector3(-1f, 1f, 0);

        // READ HIT INTFO
        if (tailHitInfo.collider != null)
        {
            ReadTailHitInfo();
            tailHitInfo = new RaycastHit();
        }

        // Update swaying mode
        prevIsSway = sway.isEnabled;
        prevIsGrab = grab.isEnabled;
    }
}
