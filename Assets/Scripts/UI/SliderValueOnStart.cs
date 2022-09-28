using UnityEngine;
using UnityEngine.UI;

public enum SliderValueType
{
    SOUND_EFFECT_VOLUME,
    MUSIC_VOLUME
}

public class SliderValueOnStart : MonoBehaviour
{
    public SliderValueType type;
    Slider slider = null;
    // Start is called before the first frame update
    void Start()
    {
        slider = GetComponent<Slider>();
        ChangeValue();
    }

    public void ChangeValue()
    {
        switch(type)
        {
            case SliderValueType.SOUND_EFFECT_VOLUME:
                slider.value = FindObjectOfType<GameMaster>().soundEffectVolume;
                break;
            case SliderValueType.MUSIC_VOLUME:
                slider.value = FindObjectOfType<GameMaster>().musicVolume;
                break;
            default:
                break;
        }
    }
}
