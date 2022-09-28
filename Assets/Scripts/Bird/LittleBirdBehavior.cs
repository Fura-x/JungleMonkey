using UnityEngine;

public enum MoveState
{
    MOTIONLESS,
    ATTACK,
    BACK,
    HIT,
    RUNAWAY
}

public enum BeginPosition
{
    START,
    END
}


public class LittleBirdBehavior : MonoBehaviour
{
    private Vector3 startPosition;
    [SerializeField] private Vector3 endPosition;
    [Header("Point position replace Vector position if set")]
    [SerializeField] private Transform endTransformPosition = null;
    [Space]
    public BeginPosition startPointPosition = BeginPosition.START;

    float currentOrientation = 0f;

    [Header("Time to cover the AB distance")]
    public float moveTime = 2f;
    private float currentMoveTime = 0f;

    [Header("Time before attack again when Player hit")]
    public float attackTimer = 1f;
    private float currentAttackTimer = 0.1f;

    public float moveHeight = 5f;

    MoveState state = MoveState.MOTIONLESS;
    [Space]
    [SerializeField] AudioSource attackSound = null;
    [SerializeField] AudioSource deathSound = null;

    public void Attack() 
    {
        state = MoveState.ATTACK;
    }

    public void PlayAttackSound()
    {
        if (attackSound != null) attackSound.Play();
    }

    public void PlayDeathSound()
    {
        if (deathSound != null) deathSound.Play();
    }

    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position;
        if (endTransformPosition != null) endPosition = endTransformPosition.position;

        if (startPointPosition is BeginPosition.END)
        {
            // SWAP POSITION
            Vector3 temp = startPosition;
            startPosition = endPosition;
            endPosition = temp;

            transform.position = startPosition;

            transform.Rotate(Vector3.up, (currentOrientation = 180f));
            GetComponent<PlayerDetection>().SwapXRange();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (currentAttackTimer > 0f)
        {
            currentAttackTimer -= Time.deltaTime;
            return;
        }

        switch (state)
        {
            case MoveState.ATTACK:
                if (currentMoveTime == 0f)
                    PlayAttackSound();

                currentMoveTime += 1/moveTime * Time.deltaTime;
                transform.position = MathParabola.ParabolaXY(startPosition, endPosition, moveHeight, currentMoveTime);

                if (currentMoveTime >= 1f)
                    ChangeMove(MoveState.BACK, false);

                break;

            case MoveState.BACK:
                currentMoveTime += 1 / moveTime * Time.deltaTime;
                transform.position = Vector3.Lerp(endPosition, startPosition, currentMoveTime);

                if (currentMoveTime >= 1f)
                    ChangeMove(MoveState.MOTIONLESS, true);

                break;
            case MoveState.HIT:
                currentMoveTime -= 1 / moveTime * Time.deltaTime;
                transform.position = MathParabola.ParabolaXY(startPosition, endPosition, moveHeight, currentMoveTime);

                if (currentMoveTime <= 0f)
                {
                    currentAttackTimer = attackTimer;
                    ChangeMove(MoveState.MOTIONLESS, true);
                }
                break;
            case MoveState.RUNAWAY:
                GetComponentInChildren<Animator>().Play("LittleBirdDeath");
                currentMoveTime += 1 / moveTime * Time.deltaTime;
                if (currentMoveTime >= 1f)
                    Destroy(gameObject);
                break;
            default:
                break;
        }
    }

    private void ChangeMove(MoveState move, bool detect)
    {
        state = move;
        currentMoveTime = 0f;
        transform.Rotate(Vector3.up, currentOrientation == 180f ? -180f : 180f);
        currentOrientation = currentOrientation == 180f ? 0f : 180f;
        GetComponent<PlayerDetection>().enabled = detect;
    }

    public void RunAway()
    {
        if (state is MoveState.RUNAWAY) return;

        state = MoveState.RUNAWAY;
        currentMoveTime = 0f;
        transform.Rotate(Vector3.up, currentOrientation == 180f ? -90f : 90f);

        GetComponent<PlayerDetection>().enabled = false;
        PlayDeathSound();
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject other = collision.gameObject;

        if (other.CompareTag("Player") && state != MoveState.RUNAWAY)
        {
            other.GetComponent<Move>().Knockback(transform);
            other.GetComponent<Life>().Hurt(1);

            if (state is MoveState.ATTACK)
            {
                state = MoveState.HIT;
                transform.Rotate(Vector3.up, 180f);
            }
        }
    }
}
