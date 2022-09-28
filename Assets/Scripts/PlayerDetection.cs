using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class PlayerDetected : UnityEvent
{ }


public class PlayerDetection : MonoBehaviour
{
    Transform player;
    Move playerMove;

    [Range(-20f, 0f)] public float xRangeNegative = -10;
    [Range(0f, 20f)] public float xRangePositive = 10;

    [Range(-20f, 0f)] public float yRangeNegative = -3;
    [Range(0f, 20f)] public float yRangePositive = 3;

    [SerializeField] private bool disableWhenDetected = true;

    public PlayerDetected OnPlayerDetected;

    void Start()
    {
        playerMove = FindObjectOfType<Move>();
        player = playerMove.transform;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 vectDistance = player.position - transform.position;

        if ( !playerMove.isInDepth
            && (vectDistance.x >= xRangeNegative && vectDistance.x <= xRangePositive)
            && (vectDistance.y >= yRangeNegative && vectDistance.y <= yRangePositive))
        {
            OnPlayerDetected.Invoke();
            enabled = !disableWhenDetected;
        }
    }

    public void SwapXRange()
    {
        float temp = xRangeNegative;
        xRangeNegative = -xRangePositive;
        xRangePositive = -temp;
    }
}
