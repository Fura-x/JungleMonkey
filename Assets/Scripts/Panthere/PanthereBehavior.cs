using UnityEngine;

public class PanthereBehavior : MonoBehaviour
{
    Rigidbody rb = null;
    Animator animator = null;

    public float yForce = 20f;
    public float zForce = 50f;
    public float frontDepthLimit = -8f;
    [Space]
    [SerializeField] bool respwan = true;
    [SerializeField] float timeToRespwan = 1f;
    float currentTimeToRespwan = 0f;
    [Space]
    [SerializeField] AudioSource attackSound = null;

    private Vector3 startPosition;
    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        startPosition = transform.position;
    }

    private void Update()
    {
        if (currentTimeToRespwan >= 0f && respwan)
        {
            currentTimeToRespwan -= Time.deltaTime;

            if (currentTimeToRespwan <= 0f)
            {
                transform.position = startPosition;
                rb.isKinematic = true;
                PlayerDetection p = GetComponents<PlayerDetection>()[1];
                p.enabled = true;
            }

            return;
        }

        if (transform.position.z <= frontDepthLimit)
        {
            currentTimeToRespwan = timeToRespwan;
            animator.SetBool("observ", true);
        }
    }

    public void Jump()
    {
        animator.Play("PanthereJump");
        animator.SetBool("observ", false);

        rb.isKinematic = false;
        rb.AddForce(new Vector3(0f, yForce, zForce), ForceMode.Impulse);
    }

    public void PlayAttackSound()
    {
        if (attackSound != null) attackSound.Play();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponentInParent<Move>().Knockback(transform);
            other.GetComponentInParent<Life>().Hurt(1);
        }
    }
}
