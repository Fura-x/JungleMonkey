using UnityEngine;

public class Bumper : MonoBehaviour
{
    public float bumpForce = 0f;

    private void Awake()
    {
        gameObject.tag = "Bumper";
    }
}
