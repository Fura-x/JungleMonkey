using UnityEngine;
using UnityEngine.EventSystems;

public class GameCanvas : MonoBehaviour
{
    new public string name;
    [SerializeField] GameObject firstSelected = null;

    public void Display()
    {
        gameObject.SetActive(true);
        FindObjectOfType<EventSystem>().SetSelectedGameObject(firstSelected);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
