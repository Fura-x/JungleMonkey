using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    MENU,
    INGAME
}

public class LevelManager : MonoBehaviour
{
    public GameState state = GameState.INGAME;

    [SerializeField] string nextLevelName = "";

    private void Start()
    {
        FindObjectOfType<GameMaster>().SceneLoaded();

        PlayerControl controls = FindObjectOfType<Move>().controls;
        controls.Player.Pause.performed += ctx => Pause();

        if (state is GameState.MENU)
            controls.Disable();
    }

    public void Pause()
    {
        CanvasManager canManager = FindObjectOfType<CanvasManager>();
        AudioManager audManager = FindObjectOfType<AudioManager>();


        if (Time.timeScale > 0f && canManager.CompareCurrentCanvas("InGame"))
        {
            canManager.SwitchCanvas("Pause");
            audManager.SetVolumeScale(audManager.pauseVolumeScale);

            Time.timeScale = 0f;
            FindObjectOfType<Move>().controls.Disable();
        }
        else if (canManager.CompareCurrentCanvas("Pause"))
        {
            canManager.SwitchCanvas("InGame");
            audManager.ResetVolumeScale();

            Time.timeScale = 1f;
            FindObjectOfType<Move>().controls.Enable();
        }

    }

    public void GoToScene(string sceneName)
    {
        Time.timeScale = 1f;

        ResetLevel();
        SceneManager.LoadScene(sceneName);
    }
    public void GoToNextLevel()
    {
        if (nextLevelName.Length > 0f)
        {
            Time.timeScale = 1f;
            ResetLevel();
            SceneManager.LoadScene(nextLevelName);
        }
    }

    public void Reload()
    {
        Time.timeScale = 1f;
        FindObjectOfType<GameMaster>().doesRespawn = true;

        string scn = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(scn);
    }

    public void ReloadAndReset()
    {
        Time.timeScale = 1f;
        ResetLevel();

        string scn = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(scn);
    }

    void ResetLevel()
    {
        GameMaster gameMaster = FindObjectOfType<GameMaster>();
        if (gameMaster != null) gameMaster.Reset();
    }


    public void Quit()
    {
        Application.Quit();
    }

    public void TranslatePlayer(Transform target)
    {
        GameObject.FindGameObjectWithTag("Player").transform.position = target.position;
    }

    public void KillPlayer()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<Life>().Hurt(3);
    }

    public void SwitchLocalizationToFrench()
    {
        FindObjectOfType<LocalizationManager>().ChangeLocale("French");
    }
    public void SwitchLocalizationToEnglish()
    {
        FindObjectOfType<LocalizationManager>().ChangeLocale("English");
    }
}
