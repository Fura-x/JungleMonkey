using UnityEngine;

public class Liana : MonoBehaviour
{
    Vector3 vecStartPosition = new Vector3();
    Vector3 vecEndPosition = new Vector3();
    [SerializeField] Transform endPosition = null;
    [Space]
    public bool replacePlayer = false;
    [SerializeField] Transform playerPosition = null;
    [Space]
    public float moveTimer = 5f;
    float currentMoveTimer = 0f;

    Transform playerTransform = null;

    private void Awake()
    {
        currentMoveTimer = 1f;

        vecStartPosition = transform.position;
        vecEndPosition = endPosition.position;
    }

    private void Start()
    {
        playerPosition.position = new Vector3(playerPosition.position.x, playerPosition.position.y, 0f);
        playerTransform = FindObjectOfType<Move>().transform;
    }

    void SwapStartAndEnd()
    {
        Vector3 temp = vecEndPosition;
        vecEndPosition = vecStartPosition;
        vecStartPosition = temp;
    }

    private void FixedUpdate()
    {
        if (currentMoveTimer >= 1f) 
            return;
        else if (replacePlayer && Vector3.Distance(playerTransform.position, playerPosition.position) >= 0.2f)
        {
            playerTransform.position = Vector3.Lerp(playerTransform.position, playerPosition.position, 0.1f);
            return;
        }

        currentMoveTimer += 1f / moveTimer * Time.fixedDeltaTime;
        transform.position = MathParabola.ParabolaXZ(vecStartPosition, vecEndPosition, 5f, currentMoveTimer);

        if (currentMoveTimer >= 1f) EndMove();

    }

    public void StartMove()
    {
        currentMoveTimer = 0f;
    }

    public void EndMove()
    {
        SwapStartAndEnd();
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<Move>().StunDisable();
        player.GetComponent<Move>().ChangeDepth();

        player.transform.SetParent(null, true);
    }

    public bool IsMoving() { return currentMoveTimer < 1f; }

    private void OnTriggerStay(Collider other)
    {
        if (!IsMoving() && other.CompareTag("Player") && other.GetComponentInParent<Move>().VerticalSitckTrigger >= 0.9f)
        {
            Move pm = other.GetComponentInParent<Move>();
            if (pm.CanChangeDepth())
            {
                playerTransform.SetParent(transform, true);
                StartMove();

                pm.Stun();
                pm.ChangeDepth();
            }
        }
    }
}
