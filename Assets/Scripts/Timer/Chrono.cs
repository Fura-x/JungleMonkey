using UnityEngine;

public class Chrono : Timer
{

    private void Start()
    {
        time = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateParent(time);
    }
}
