using UnityEngine;
using UnityEngine.UI;

public enum SliderDisplaymode
{
    PERCENT,
    NORMAL
}


public class SliderValueDisplay : MonoBehaviour
{
    [SerializeField] Slider target = null;
    Text text = null;

    public SliderDisplaymode display = SliderDisplaymode.PERCENT;

    void Awake()
    {
        text = GetComponent<Text>();
        UpdateText();
    }

    public void UpdateText()
    {
        if (target is null) return;

        switch (display)
        {
            case SliderDisplaymode.NORMAL:
                text.text = target.value.ToString("0.00");
                break;
            default:
                float percent = target.value * 100f / target.maxValue;
                percent = Mathf.Ceil(percent / 10f) * 10f;
                text.text = ((int)percent).ToString() + "%";
                break;
        }
    }
}
