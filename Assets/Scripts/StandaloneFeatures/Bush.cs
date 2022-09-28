using UnityEngine;

public class Bush : MonoBehaviour
{
    Collider thisColliderFront = null;
    Collider thisColliderInside = null;
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
            tempCollider.radius = radius * 0.8f;
            tempCollider.center = new Vector3(center.x, center.y, (0f - transform.position.z));

            thisColliderFront = tempCollider;
            thisColliderFront.isTrigger = true;

            // Inside
            tempCollider = gameObject.AddComponent<SphereCollider>();
            tempCollider.radius = radius;
            tempCollider.center = new Vector3(center.x, center.y, 0f);

            thisColliderInside = tempCollider;
            thisColliderInside.isTrigger = true;
        }

        gameObject.tag = "Bush";
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && other.GetComponentInParent<Move>().VerticalSitckTrigger != 0f)
        {
            Move pm = other.GetComponentInParent<Move>();

            if (pm.CanChangeDepth() && pm.isInDepth && pm.VerticalSitckTrigger <= -0.9f)
            {
                other.GetComponentInParent<Move>().StunDisable();

                Vector3 pos = other.transform.parent.position;
                other.transform.parent.position = new Vector3(pos.x, pos.y, 0f);

                pm.ChangeDepth();   
            }
            else if(pm.CanChangeDepth() && !pm.isInDepth && pm.VerticalSitckTrigger >= 0.9f)
            {
                other.GetComponentInParent<Move>().Stun();

                Vector3 pos = other.transform.parent.position;
                other.transform.parent.position = new Vector3(pos.x, pos.y, transform.position.z);

                pm.ChangeDepth();
            }
        }
    }
}