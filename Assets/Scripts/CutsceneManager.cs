using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneManager : MonoBehaviour
{
    public bool LevelIntro = true;
    [SerializeField] GameObject player = null;
    [SerializeField] GameObject cam = null;
    private GameMaster gm;
    private void Start()
    {
        gm = GameObject.FindGameObjectWithTag("CheckpointMaster").GetComponent<GameMaster>();
        if(!gm.doesRespawn && LevelIntro)
        {
            PlayAnimation();
        }
        else if (gm.doesRespawn && LevelIntro)
        {
            SwitchToPlayer();
        }
    }

    public void PlayAnimation()
    {
        gameObject.GetComponent<Animation>().Play();
        //player.GetComponent<Move>().controls.Player.Run.performed += ctx => StopAnimation();
    }

    public void StopAnimation()
    {
        gameObject.GetComponent<Animation>().Stop();
        //player.GetComponent<Move>().controls.Player.Run.performed -= ctx => StopAnimation();

        if (LevelIntro) SwitchToPlayer();
        else SwitchToLevelComplete();
    }

    private void SwitchToPlayer()
    {
        player.GetComponent<CutsceneManagement>().Enable();
        cam.GetComponent<FollowingCamera>().SetNewTarget(player.transform);
    }

    private void SwitchToLevelComplete()
    {
        FindObjectOfType<CanvasManager>().SwitchCanvas("GameSuccess");
        Time.timeScale = 0f;
    }
}
