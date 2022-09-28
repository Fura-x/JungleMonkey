using System.Collections.Generic;
using System.IO;
using System.Text;

using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class LocalizationUpdateEvent : UnityEvent<Locale>
{ }

public class LocalizationManager : MonoBehaviour
{
    private static LocalizationManager instance = null;

    Dictionary<string, Locale> locales = new Dictionary<string, Locale>();

    string fileName = "config";
    string localesPath;

    public string currentLocale { get; private set; }
    [HideInInspector] public LocalizationUpdateEvent OnLocalizationChange;

    public Locale GetLocale() { return locales[currentLocale]; }
    // Start is called before the first frame update
    private void Awake()
    {
        if (instance is null)
        {
            instance = this;
            DontDestroyOnLoad(instance);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        localesPath = "Localization/";

        TextAsset config = Resources.Load<TextAsset>(localesPath + fileName);
        byte[] byteArray = Encoding.ASCII.GetBytes(config.text);
        MemoryStream stream = new MemoryStream(byteArray);
        StreamReader configStream = new StreamReader(stream);
        //localesPath = Resources.Load<StreamReader>("Localization/");// Application.dataPath + "/Resources/Localization/";

        {
            // Read first line with locales list
            string line = configStream.ReadLine();
            string[] languages = line.Split('|');
            foreach (string language in languages)
            {
                if (language == string.Empty) continue;

                // Search locale txt file
                string localePath = localesPath + language;
                // Create new locale
                if (currentLocale is null) currentLocale = language;
                locales.Add(language, new Locale(localePath));
            }
        }
        configStream.Close();
    }

    public bool CanOpenFile(in string filePath)
    {
        if (!File.Exists(filePath))
        {
            print("Can't find " + filePath);
            return false;
        }

        return true;
    }

    public string GetValue(string key)
    {
        return locales[currentLocale].GetValue(key);
    }

    public void ChangeLocale(string newLocale)
    {
        if (!locales.ContainsKey(newLocale)) return;

        currentLocale = newLocale;
        OnLocalizationChange.Invoke(locales[currentLocale]);
    }
}
