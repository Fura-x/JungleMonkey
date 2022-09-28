using UnityEngine;

public class DepthDoor : MonoBehaviour
{
    Collider thisColliderFront = null;
    Collider thisColliderBack = null;
    [SerializeField] bool isColliderSet = false;
    [Header("If collider not set, use these variables : ")]
    [SerializeField] float radius = 1f;
    [SerializeField] Vector2 center = new Vector2();


    void Start()
    {
        if (!isColliderSet)
        {
            // FRONT
            SphereCollider tempCollider = gameObject.AddComponent<SphereCollider>();
            tempCollider.radius = radius;
            tempCollider.center = new Vector3(center.x, center.y, (0f - transform.position.z));

            thisColliderFront = tempCollider;
            thisColliderFront.isTrigger = true;

            // BACK
            float depth = FindObjectOfType<Move>().defaultDepth;
            tempCollider = gameObject.AddComponent<SphereCollider>();
            tempCollider.radius = radius;
            tempCollider.center = new Vector3(center.x, center.y, depth-transform.position.z);

            thisColliderBack = tempCollider;
            thisColliderBack.isTrigger = true;
        }   

        gameObject.tag = "DepthDoor";
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && other.GetComponentInParent<Move>().VerticalSitckTrigger != 0f)
        {
            Move pm = other.GetComponentInParent<Move>();
            if (pm.CanChangeDepth() && pm.isInDepth && pm.VerticalSitckTrigger <= -0.9f)
            {
                Vector3 pos = other.transform.parent.position;
                other.transform.parent.position = new Vector3(pos.x, pos.y, 0f);

                pm.ChangeDepth();
            }
            else if(pm.CanChangeDepth() && !pm.isInDepth && pm.VerticalSitckTrigger >= 0.9f)
            {
                Vector3 pos = other.transform.parent.position;
                other.transform.parent.position = new Vector3(pos.x, pos.y, pm.defaultDepth);

                pm.ChangeDepth();
            }
        }
    }
}
