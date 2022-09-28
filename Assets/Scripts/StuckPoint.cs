using UnityEngine;

public class StuckPoint : MonoBehaviour
{
    [SerializeField] Transform stuckPoint = null;

    public Transform GetStuckTransform() { return stuckPoint; }
    public void SetStuckTransform(Transform point) { stuckPoint = point; }

    void FixedUpdate()
    {
        if (stuckPoint != null)
        {
            stuckPoint.position = transform.position;
        }
    }
}
