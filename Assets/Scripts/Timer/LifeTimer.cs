using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class LifeTimerEnded : UnityEvent
{

}

public class LifeTimer : Timer
{
    [Header("LifeTime settings")]
    [Tooltip("In secondes")]
    [SerializeField] float lifeTime = 0f;

    public LifeTimerEnded OnTimerEnded;
    // Update is called once per frame
    void Update()
    {
        lifeTime -= Time.deltaTime;

        if (IsTimerEnded()) OnTimerEnded.Invoke();

        UpdateParent(lifeTime);
    }

    public bool IsTimerEnded()
    {
        return lifeTime <= 0f;
    }
}
