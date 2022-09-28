using UnityEngine;

public class FollowingCamera : MonoBehaviour
{
    [SerializeField]
    private Transform target = null;

    [SerializeField] Vector3 defaultOffset = new Vector3();
    Vector3 offset;
    [Range(0f, 1f)] public float smooth;

    public void SetNewTarget(Transform newTarget)
    {
        target = newTarget;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (!target) target = GameObject.FindGameObjectWithTag("Player").transform;
        offset = defaultOffset;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (target is null) return;

        transform.position = Vector3.Lerp(transform.position, target.position + offset, smooth);
    }


    public void SetNewOffset(Vector3 newOffset)
    {
        offset = newOffset;
    }

    public void SetNewOffsetX(float newX)
    {
        offset.x = newX;
    }

    public void SetNewOffsetY(float newY)
    {
        offset.y = newY;
    }
    public void SetNewOffsetZ(float newZ)
    {
        offset.z = newZ;
    }

    public void AddToOffset(Vector3 additiveOffset)
    {
        offset = defaultOffset + additiveOffset;
    }

    public void ResetOffset()
    {
        offset = defaultOffset;
    }
}
