using UnityEngine;

[RequireComponent(typeof(CobraBehaviour))]
public class CobraPath : MonoBehaviour
{
    [SerializeField] private Transform[] path = null;
    [SerializeField] private float moveSpeed = 5;
    int waypointID = 0;
    Rigidbody rb;
    Vector3 direction = new Vector3(0, 0, 0);
    Vector3 previousWayPoint;

    CobraBehaviour behaviour = null;

    // Start is called before the first frame update
    void Awake()
    {
        previousWayPoint = path[0].position;
        transform.position = path[waypointID].transform.position;
        rb = gameObject.GetComponent<Rigidbody>();
        behaviour = GetComponent<CobraBehaviour>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (!behaviour.IsAttacking() && !behaviour.IsStunned())
        {
            rb.velocity = direction * moveSpeed;
            transform.Translate(direction * moveSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, previousWayPoint) >=
                Vector3.Distance(previousWayPoint, path[waypointID].position))
            {
                previousWayPoint = path[waypointID].position;
                transform.position = previousWayPoint;
                waypointID = (waypointID + 1) % path.Length;
                direction = (path[waypointID].position - transform.position).normalized;
            }
        }
    }
}