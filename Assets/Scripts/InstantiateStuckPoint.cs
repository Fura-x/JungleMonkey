using UnityEngine;

public class InstantiateStuckPoint : MonoBehaviour
{
    [SerializeField] GameObject point = null;
    [SerializeField] StuckPoint stuck = null;
    // Start is called before the first frame update
    void Start()
    {
        GameObject newPoint = Instantiate(point, null, true);
        stuck.SetStuckTransform(newPoint.transform);
    }
}
