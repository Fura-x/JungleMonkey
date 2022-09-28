using System.Collections.Generic;
using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    [SerializeField] List<GameCanvas> canvasList = new List<GameCanvas>();
    [Space]
    [SerializeField] GameCanvas currentCanvas = null;
    [SerializeField] GameCanvas gameUICanvas = null;

    // Start is called before the first frame update
    void Awake()
    {
        foreach (GameCanvas can in canvasList)
            can.Hide();

        if (gameUICanvas != null) gameUICanvas.Display();
        if (currentCanvas != null) 
            currentCanvas.Display();
    }

    public void SwitchCanvas(string name)
    {
        currentCanvas.Hide();
        GameCanvas temp = canvasList.Find(can => can.name == name);
        if (temp != null)
        {
            currentCanvas = temp;
            currentCanvas.Display();
        }
    }

    public void Display(string name)
    {
        GameCanvas temp = canvasList.Find(can => can.name == name);
        if (temp != null) temp.Display();
    }

    public void Hide(string name)
    {
        GameCanvas temp = canvasList.Find(can => can.name == name);
        if (temp != null) temp.Hide();
    }

    public bool CompareCurrentCanvas(string nameToCompare)
    {
        return currentCanvas.name.Equals(nameToCompare);
    }
}
