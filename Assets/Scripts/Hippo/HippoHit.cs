using UnityEngine;

public class HippoHit : MonoBehaviour
{
    [SerializeField] Animator animator = null;
    //[SerializeField] Transform hippoRootTransform = null;

    private bool isReturn = false;

    [SerializeField] private float maxKOTimer = 8f;
    private float KOTimer = 0f;


    private void Update()
    {
        if (KOTimer > 0f)
        {
            KOTimer -= Time.deltaTime;

            if (KOTimer <= 0f)
                WakeUp();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Collider other = collision.collider;
        Collider thisCollider = collision.contacts[0].thisCollider;

        if (!isReturn && other.CompareTag("Tail") && thisCollider.gameObject.CompareTag("WeakPoint"))
        {
            KnockOut();
        }
    }

    public void KnockOut()
    {
        if (isReturn) return;

        animator.SetBool("isWakingUp", false);
        animator.Play("HippoReturns");

        HippoBehavior hp = GetComponent<HippoBehavior>();
        hp.SetMoving(false);
        hp.PlayKOSound();

        GetComponent<Rigidbody>().isKinematic = true;
        isReturn = true;

        KOTimer = maxKOTimer;
    }

    public void WakeUp()
    {
        animator.SetBool("isWakingUp", true);

        GetComponent<HippoBehavior>().SetMoving(true);
        GetComponent<Rigidbody>().isKinematic = false;
        isReturn = false;
    }
}
