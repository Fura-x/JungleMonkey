using UnityEngine;

public class BigBirdBehavior : MonoBehaviour
{
    [Header("Move on start or not")]
    public bool isMoving = true;

    [SerializeField] private Vector3 startPosition;
    [SerializeField] private Vector3 endPosition;
    [Header("Point position replace Vector3 position if set")]
    [SerializeField] private Transform startPointPosition = null;
    [SerializeField] private Transform endPointPosition = null;
    [Space]

    public float speed = 3f;
    private float lastSpeed = 3f;

    private float maxTimer = 0f;
    private float timer = 0f;

    public float stunTimer = 1f;
    private float currentStunTimer = 0f;
    [SerializeField] GameObject StunAnim = null;

    public void StartMove()
    {
        isMoving = true;
    }

    // Start is called before the first frame update
    void Awake()
    {
        // If transform is associate, 
        if (startPointPosition != null) startPosition = startPointPosition.position;
        if (endPointPosition != null) endPosition = endPointPosition.position;
        transform.position = startPosition;

        lastSpeed = speed;
        maxTimer = Vector3.Distance(startPosition, endPosition) / speed;

        // Rotate Bird if go to the right
        if (Mathf.Sign((endPosition - startPosition).x) == 1f) transform.Rotate(Vector3.up, 180f);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!isMoving) return;

        if (currentStunTimer > 0f)
        {
            currentStunTimer -= Time.fixedDeltaTime;
            if (currentStunTimer <= 0f)
            {
                timer = 1f - timer;
                Rotate();
                if (StunAnim != null) StunAnim.SetActive(false);
            }
            return;
        }

        UpdateSpeed();

        timer += 1 / maxTimer * Time.fixedDeltaTime;
        transform.position = Vector3.Lerp(startPosition, endPosition, timer);

        if (timer >= 1f)
        {
            timer = 0f;
            Rotate();
        }
    }

    void UpdateSpeed()
    {
        // Useful if Someone change speed in inspector
        if (speed != lastSpeed)
        {
            lastSpeed = speed;
            maxTimer = Vector3.Distance(startPosition, endPosition) / speed;
        }
    }

    void Rotate()
    {
        transform.Rotate(Vector3.up, 180f);
        FindObjectOfType<Move>().BigBirdRotate(GetComponent<StuckPoint>().GetStuckTransform());
        // Swap
        Vector3 temp = startPosition;
        startPosition = endPosition;
        endPosition = temp;
    }

    public void Stun()
    {
        currentStunTimer = stunTimer;
        if (StunAnim != null) StunAnim.SetActive(true);
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject other = collision.gameObject;

        if (other.CompareTag("Player"))
        {
            Collider thisCollider = collision.contacts[0].thisCollider;
            if (thisCollider.tag.Contains("Beak"))
            {
                other.GetComponent<Move>().Knockback(transform);
                other.GetComponent<Life>().Hurt(1);
            }
        }
    }
}
