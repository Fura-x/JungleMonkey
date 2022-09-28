using UnityEngine;

public class FreezeYWhenReachFloor : MonoBehaviour
{
    Rigidbody rb = null;

    bool isGrounded = false;
    public bool lockExit = false;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = 
                RigidbodyConstraints.FreezeRotation
                | RigidbodyConstraints.FreezePositionZ;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!enabled || isGrounded) return;

        Collider other = collision.collider;

        if (other.CompareTag("Floor"))
        {
            isGrounded = true;
            rb.constraints = 
                RigidbodyConstraints.FreezeRotation
                | RigidbodyConstraints.FreezePositionY 
                | RigidbodyConstraints.FreezePositionZ;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (!enabled || !isGrounded || lockExit) return;

        Collider other = collision.collider;

        if (other.CompareTag("Floor"))
        {
            isGrounded = false;
            rb.constraints =
                RigidbodyConstraints.FreezeRotation
                | RigidbodyConstraints.FreezePositionZ;
        }
    }
}
