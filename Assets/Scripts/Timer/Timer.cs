using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    protected float time = 0f;

    [SerializeField] protected Text text = null;
    public GameObject[] timerPanelToEnable;

    private void Start()
    {
        foreach (GameObject panel in timerPanelToEnable)
            panel.SetActive(true);
    }

    // Update is called once per frame
    protected void UpdateParent(float time)
    {
        this.time += Time.deltaTime;

        if (text != null) UpdateText(time);
    }

    private void UpdateText(float time)
    {
        float minutes = GetMinutes(time);
        int secondes = GetSecondes(minutes, time);

        text.text = minutes.ToString("0") + " : " + (secondes > 9 ? secondes.ToString() : "0" + secondes.ToString());
    }

    private float GetMinutes(float secondes)
    {
        return Mathf.Floor(secondes / 60f);
    }

    private int GetSecondes(float minutes, float totalSecondes)
    {
        return Mathf.FloorToInt(totalSecondes - minutes * 60f);
    }

    public override string ToString()
    {
        float minutes = GetMinutes(time);
        int secondes = GetSecondes(minutes, time);

        return minutes.ToString("0") + " : " + (secondes > 9 ? secondes.ToString() : "0" + secondes.ToString());
    }
}
