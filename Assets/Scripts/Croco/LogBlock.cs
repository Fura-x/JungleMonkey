using UnityEngine;

public class LogBlock : MonoBehaviour
{
    bool isBlocked = false;
    GameObject log = null;
    Animation anim = null;
    private void Awake()
    {
        log = transform.Find("Log").gameObject;
        anim = GetComponentInChildren<Animation>();
    }
    private void OnCollisionEnter(Collision collision)
    { 
        if(collision.gameObject.tag == "Log" && !isBlocked)
        {
            log.GetComponent<CapsuleCollider>().enabled = true;
            log.GetComponent<MeshRenderer>().enabled = true;
            isBlocked = true;
            Destroy(collision.gameObject);
        }
    }
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "Player" && !isBlocked)
        {
            collision.gameObject.GetComponent<Move>().Knockback(transform);
            collision.gameObject.GetComponent<Life>().Hurt(1);
            if(!anim.isPlaying)
            {
                anim.Play("CrocoMunch");
            }
        }
    }
}
