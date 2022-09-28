using UnityEngine;
using UnityEngine.Events;


[System.Serializable]
public class LocaleUpdateEvent : UnityEvent<string>
{ }

public class LocalizeText : MonoBehaviour
{
    public string key = "DEFAULT";

    public LocaleUpdateEvent textToUpdate;

    private void Start()
    {
        LocalizationManager manager = FindObjectOfType<LocalizationManager>();
        manager.OnLocalizationChange.AddListener(UpdateText);
        UpdateText(manager.GetLocale());
    }
    public void UpdateText(Locale locale)
    {
        textToUpdate.Invoke(locale.GetValue(key is null ? "DEFAULT" : key));
    }
}
