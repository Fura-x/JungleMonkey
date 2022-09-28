using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSecretsInfo
{
    public LevelSecretsInfo() { }
    public LevelSecretsInfo(string name)
    {
        this.name = name;
    }

    public string name;

    public int currentScore = 0;
    public int maxScore = 0;
}

public class GameMaster : MonoBehaviour
{
    private static GameMaster instance;

    [Header("Variable use in Runtime to know if Load scene after Respawn or not")]
    public bool doesRespawn = false;

    // CHECKPOINT
    public Vector3 lastCheckpointPos = Vector3.zero;

    // SOUNDS
    public float musicVolume = 1f;
    public float soundEffectVolume = 1f;

    // SECRETS
    List<LevelSecretsInfo> levelScore = new List<LevelSecretsInfo>();
    string currentScene;
    [HideInInspector] public List<GameObject> secrets;
    public int collectedSecret { get; private set; }


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
        }
    }

    public void SceneLoaded()
    {
        currentScene = SceneManager.GetActiveScene().name;
        if (FindLevelSecretInfo(currentScene) is null)
            levelScore.Add(new LevelSecretsInfo(currentScene));
    }

    public void Reset()
    {
        lastCheckpointPos = new Vector3();
        secrets.Clear();
        doesRespawn = false;
    }

    public int CollectedSecretCount(bool levelCompleted)
    {
        int score = 0;
        foreach (GameObject secret in secrets)
        {
            if (!secret.activeInHierarchy)
                score++;
        }

        // Get new HighScore
        if (levelCompleted)
        {
            LevelSecretsInfo lvl = FindLevelSecretInfo(currentScene);
            int scoreDifference = score - lvl.currentScore;

            collectedSecret += scoreDifference <= 0 ? 0 : scoreDifference;
            lvl.currentScore = score;
            lvl.maxScore = secrets.Count;
        }

        return score;
    }

    public LevelSecretsInfo FindLevelSecretInfo(string name)
    {
        return levelScore.Find(level => name.Equals(level.name));
    }
}
