using UnityEngine;
using UnityEngine.UI;

public class LockSecretButton : MonoBehaviour
{
    [SerializeField] int secretToUnlock = 10;
    [SerializeField] GameObject unlockButton = null;
    [TextArea (1, 2)] public string defaultText;

    private void OnEnable()
    {
        int currentSecretCollected = FindObjectOfType<GameMaster>().collectedSecret;
        if (secretToUnlock <= currentSecretCollected) Unlock();
        else Lock() ;
    }

    public void Unlock() 
    {
        GetComponent<Button>().interactable = true;
        if (unlockButton != null) unlockButton.SetActive(false);
    }

    public void Lock()
    {
        int currentSecretCollected = FindObjectOfType<GameMaster>().collectedSecret;

        GetComponent<Button>().interactable = false;
        GetComponentInChildren<Text>().text = defaultText + "\nSecrets : " + currentSecretCollected.ToString() + "/" + secretToUnlock.ToString();
        if (unlockButton != null) unlockButton.SetActive(true);
    }
}
