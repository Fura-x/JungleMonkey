using UnityEngine;
using UnityEngine.Events;

[System.Serializable]

public class EndTriggerEvent : UnityEvent
{ }

public class LevelEndTrigger : MonoBehaviour
{
    public EndTriggerEvent OnTriggered;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            OnTriggered.Invoke();
            other.GetComponentInParent<Move>().controls.Disable();
        }
    }
}
