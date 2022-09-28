using UnityEngine;

public enum Orientation
{
    RIGHT,
    LEFT
}

public class RhinoBehavior : MonoBehaviour
{
    Rigidbody rb = null;
    public float knockBackForce = 10f;
    public Orientation orientation = Orientation.RIGHT;

    // SPEED and DISTANCE
    [Header("Movement settings")]
    [Range(0f, 1f)] public float Acceleration;
    public float maxSpeed = 10f;
    float currentSpeed = 0f;
    public float maxDistance = 10f;
    float currentDistance = 0f;

    float lastX = 0f;

    // WAIT TIMERS
    [Header("Wait settings")]
    public float chargeWaitTimer = 2f;
    float currentWaitTimer = 0f;

    bool isStun = false;
    public float stunTimer = 3f;
    float currentStunTimer = 0f;
    [SerializeField] GameObject stunAnim = null;

    public float lookAtPlayerTimer = 2f;
    float currentLookAtTimer = 0f;

    // MOVE INFO
    private bool forceMoving = false;
    private bool isMoving = false;

    [Header("Sound")]
    [SerializeField] AudioSource rhinoYellSound = null;
    [SerializeField] AudioSource rhinoImpactSound = null;
    [SerializeField] AudioSource rhinoChargeSound = null;
    int yellPriority = 0;
    [Header("Value that reduces rhino sound's priority when charge")]
    [Range(0, 128)] [SerializeField] int yellPriorityReducing = 0;

    public void StartMove() 
    {
        currentLookAtTimer = lookAtPlayerTimer;
        currentWaitTimer = chargeWaitTimer;
        currentDistance = 0f;
        isMoving = true;
        if (rb != null) rb.isKinematic = false;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        lastX = transform.position.x;
        currentWaitTimer = chargeWaitTimer;
        currentLookAtTimer = lookAtPlayerTimer;

        if (orientation is Orientation.LEFT)
        {
            transform.Rotate(Vector3.up, 180f);
            GetComponent<PlayerDetection>().SwapXRange();
        }

        if (rhinoYellSound != null) yellPriority = rhinoYellSound.priority;
    }

    // Update is called once per frame
    void Update()
    {
        // STUN
        if (currentStunTimer > 0f)
        {
            currentStunTimer -= Time.deltaTime;
            if (currentStunTimer <= 0f)
            {
                ChangeOrientation();

                if (isStun)
                {
                    rb.isKinematic = isStun = false;
                    stunAnim.SetActive(false);
                }
            }
            return;
        }
        // CAN MOVE 
        if (!isMoving) return;

        if (!forceMoving)
        {
            // LOOKAT
            if (!LookAtPlayer())
            {
                StopChargeSound();
                return;
            }
            // WAIT BEFORE CHARGE
            else if (currentWaitTimer > 0f)
            {
                currentWaitTimer -= Time.deltaTime;

                if (currentWaitTimer <= 0f) PlayChargeSound();
                return;
            }
        }

        // Movement
        currentSpeed = Mathf.Lerp(currentSpeed, maxSpeed, Acceleration);
        rb.velocity = new Vector3(currentSpeed * transform.right.x, rb.velocity.y, 0f);

        // Distance
        currentDistance += transform.position.x - lastX;
        if (Mathf.Abs(currentDistance) >= maxDistance)
        {
            StopCharge();
            currentStunTimer = 0.5f;
        }

        lastX = transform.position.x;
    }

    public bool LookAtPlayer()
    {
        Vector3 playerPos = GameObject.FindGameObjectWithTag("Player").transform.position;

        float xDist = playerPos.x - transform.position.x;
        // Turn around toward player
        if (currentLookAtTimer <= 0f)
        {
            ChangeOrientation();
            return true; ;
        }
        // Detect that player is behind, and wait a moment
        else if ((orientation is Orientation.RIGHT && xDist < 0f)
            || (orientation is Orientation.LEFT && xDist >= 0f))
        {
            currentLookAtTimer -= Time.deltaTime;
            return false;
        }
        // Player is in front of it
        else return true;
    }

    private void StopCharge()
    {
        isMoving = forceMoving = false;

        GetComponent<PlayerDetection>().enabled = true;
        StopChargeSound();
    }

    public void ForceCharge()
    {
        PlayChargeSound();
        isMoving = forceMoving = true;
    }

    private void ChangeOrientation()
    {
        transform.Rotate(Vector3.up, 180f);
        transform.Translate(Vector3.left * 1.2f);
        orientation = orientation is Orientation.RIGHT ? Orientation.LEFT : Orientation.RIGHT;

        currentLookAtTimer = lookAtPlayerTimer;
        currentWaitTimer = chargeWaitTimer;

        GetComponent<PlayerDetection>().SwapXRange();
    }

    private void OnCollisionStay(Collision collision)
    {
        if (!isMoving || isStun || !enabled) return;

        GameObject other = collision.gameObject;

        if (collision.contacts[0].thisCollider.tag.Contains("Head"))
        {
            if (other.CompareTag("Player"))
            {
                other.GetComponent<Move>().Knockback(transform);
                other.GetComponent<Life>().Hurt(1);

                rb.velocity = Vector3.zero;
                rb.AddForce(-transform.right * knockBackForce, ForceMode.Impulse);

                currentStunTimer = 0.2f;
                StopCharge();
            }
            else
            {
                transform.Translate(transform.right * 1.05f, Space.World);

                currentStunTimer = stunTimer;
                isStun = rb.isKinematic = true;

                stunAnim.SetActive(true);
                StopCharge();

                PlayImpactSound();
            }
        }
    }

    public void PlayImpactSound()
    {
        StopChargeSound();
        if (rhinoImpactSound != null) rhinoImpactSound.Play();
    }

    public void PlayChargeSound()
    {
        if (rhinoChargeSound != null)
        {
            rhinoChargeSound.Play();
            if (rhinoYellSound != null) rhinoYellSound.priority = yellPriority - yellPriorityReducing;
        }
    }
    public void StopChargeSound()
    {
        if (rhinoChargeSound != null)
        {
            rhinoChargeSound.Stop();
            if (rhinoYellSound != null) rhinoYellSound.priority = yellPriority;

        }
    }
}
