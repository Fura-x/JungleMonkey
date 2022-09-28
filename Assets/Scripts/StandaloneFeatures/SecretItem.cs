using UnityEngine;
using UnityEngine.SceneManagement;

public class SecretItem : MonoBehaviour
{
    bool isInstantiate = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gameObject.SetActive(false);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // If load a new Scene, set new secret as DontDestroy, and destroy previous secret
        if (!FindObjectOfType<GameMaster>().doesRespawn)
        {
            if (isInstantiate)
                Destroy(gameObject);
            else
            {
                DontDestroyOnLoad(gameObject);
                FindObjectOfType<GameMaster>().secrets.Add(gameObject);
                isInstantiate = true;

                SceneManager.sceneLoaded += OnSceneLoaded;
            }
        }
        // If reload the current scene and not instantiate, destroy the copy
        else if (!isInstantiate)
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
