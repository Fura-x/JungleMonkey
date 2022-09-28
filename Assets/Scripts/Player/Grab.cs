using UnityEngine;

public class Grab : MonoBehaviour
{
    Rigidbody rb = null;

    public bool isEnabled = false;
    Vector3 grabPoint;

    [SerializeField] float grabSpeed = 5f;

    public void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void SelfUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, grabPoint, grabSpeed);
    }

    public void Enable(Vector3 grabPoint)
    {
        // Disable movement and activate grab
        isEnabled = true;
        this.grabPoint = grabPoint;
        // Disable Rigidbody gravity
        rb.velocity = Vector3.zero;
        rb.useGravity = false;
    }

    public void Disable()
    {
        // Apply method only if activate
        if (!isEnabled) return;

        isEnabled = false;
        rb.useGravity = true;
    }
}
