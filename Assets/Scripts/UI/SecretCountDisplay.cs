using UnityEngine;
using UnityEngine.UI;

public enum SecretCountDisplayMode
{
    MENU,
    LEVEL,
    LEVEL_END
}

public class SecretCountDisplay : MonoBehaviour
{
    public SecretCountDisplayMode mode = SecretCountDisplayMode.LEVEL_END;

    Text text = null;
    // Start is called before the first frame update
    void OnEnable()
    {
        text = GetComponent<Text>();
        text.text = SecretCountToString();
    }

    string SecretCountToString()
    {
        GameMaster master = FindObjectOfType<GameMaster>();

        switch(mode)
        {
            case SecretCountDisplayMode.MENU:
                if (master.collectedSecret == 0)
                {
                    transform.parent.gameObject.SetActive(false);
                    return "";
                }
                else
                {
                    transform.parent.gameObject.SetActive(true);
                    return "Secrets : " + master.collectedSecret.ToString();
                }
            case SecretCountDisplayMode.LEVEL:
                if (master.secrets.Count == 0)
                {
                    transform.parent.gameObject.SetActive(false);
                    return "";
                }
                return "Secrets : " + master.CollectedSecretCount(false).ToString() + " / " + master.secrets.Count.ToString();
            default:
                if (master.secrets.Count == 0)
                {
                    transform.parent.gameObject.SetActive(false);
                    return "";
                }
                return "Secrets : " + master.CollectedSecretCount(true).ToString() + " / " + master.secrets.Count.ToString();
        }
    }
}
