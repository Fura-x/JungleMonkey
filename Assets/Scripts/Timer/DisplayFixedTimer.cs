using UnityEngine;
using UnityEngine.UI;

public class DisplayFixedTimer : MonoBehaviour
{
    [SerializeField] Timer timer = null;

    private void OnEnable()
    {
        if (timer is null && !timer.isActiveAndEnabled) return;

        GetComponent<Text>().text = "Your time " + timer.ToString();
    }
}
