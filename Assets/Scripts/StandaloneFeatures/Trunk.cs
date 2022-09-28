using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trunk : MonoBehaviour
{
    [SerializeField] Animation anim = null;
    bool isBroken = false;
    private void OnCollisionEnter(Collision collision)
    {
        Collider other = collision.collider;

        if (!isBroken && other.CompareTag("Enemy_Rhino_Head"))
        {
            Transform otherTransform = collision.gameObject.transform;
            otherTransform.Translate(otherTransform.right * 1.2f, Space.World);
            otherTransform.SetParent(transform, true);

            if (other.transform.position.x < transform.position.x) anim.Play("TrunkFallRight");
            else anim.Play("TrunkFall");

            collision.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            RhinoBehavior rhb = collision.gameObject.GetComponent<RhinoBehavior>();
            rhb.enabled = false;
            rhb.PlayImpactSound();

            Destroy(collision.gameObject.GetComponent<Rigidbody>());

            isBroken = true;
        }
    }
}
