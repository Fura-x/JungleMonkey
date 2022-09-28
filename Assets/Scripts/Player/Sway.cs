using UnityEngine;

public class Sway : MonoBehaviour
{
    SpringJoint joint = null;
    public bool isEnabled = false;
    [Header("Distance use by Joint :")]
    float swayDistance = .8f;

    [Header("Distance use to retract tail :")]
    float distanceFromHit = 0f;
    [HideInInspector] public float maxDistanceFromHit = 0f;
    [HideInInspector] public float minDistanceFromHit = 0.5f;
    [Space]
    [SerializeField] float swaySpring = 50f;
    [SerializeField] float swayDamper = 7f;
    [SerializeField] float swayMassScale = 4.5f;
    [Space]
    [SerializeField] [Range(0f, 1f)] float joyStickSensibility = 0.9f;
    [SerializeField] float retractSpeed = 1;

    public bool isRetractable = true;
    private float distanceChanger = 0f;

    // Start is called before the first frame update
    public void Enable(Vector3 grabPoint, Rigidbody hitRb)
    {
        joint = gameObject.AddComponent<SpringJoint>();
        joint.autoConfigureConnectedAnchor = false;
        joint.connectedAnchor = grabPoint;

        distanceFromHit = Vector3.Distance(transform.position, grabPoint);

        joint.maxDistance = joint.minDistance = distanceFromHit * swayDistance;

        joint.spring = swaySpring;
        joint.damper = swayDamper;
        joint.massScale = swayMassScale;

        isEnabled = true;
    }

    private void Update()
    {
        if (!isEnabled || distanceChanger == 0f) return;

        if (distanceFromHit <= maxDistanceFromHit && distanceFromHit >= minDistanceFromHit)
        {
            distanceFromHit += distanceChanger * retractSpeed * Time.deltaTime;
        }
        else if (distanceFromHit > maxDistanceFromHit)
        {
            distanceFromHit = maxDistanceFromHit;
        }
        else if (distanceFromHit < minDistanceFromHit)
        {
            distanceFromHit = minDistanceFromHit;
        }

        joint.maxDistance = joint.minDistance =  distanceFromHit * swayDistance;
    }

    public void ChangeSize(float ctx)
    {
        if (Mathf.Abs(ctx) > joyStickSensibility)
        {
            distanceChanger = Mathf.Sign(ctx);
        }
        else distanceChanger = 0;
    }

    public void Disable()
    {
        // Apply method only if activate
        if (!isEnabled) return;

        Destroy(joint);
        isEnabled = false;
        distanceChanger = 0f;
    }
}
