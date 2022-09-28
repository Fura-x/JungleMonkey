using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Move))]
public class CutsceneManagement : MonoBehaviour
{
    Move move;
    private void Awake()
    {
        move = GetComponent<Move>();
    }
    public void Enable()
    {
        move.enabled = true;
    }
    public void Disable()
    {
        move.enabled = false;
    }
}
