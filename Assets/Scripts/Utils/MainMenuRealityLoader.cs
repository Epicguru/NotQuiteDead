using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuRealityLoader : MonoBehaviour
{
    public string RealityName;
    public bool Host;

    public void Start ()
	{
        DontDestroyOnLoad(this.gameObject);
        Debug.Log("Start, current scene is " + SceneManager.GetActiveScene().name);

        SceneManager.activeSceneChanged += SceneChange;
	}

    public void SceneChange(Scene oldScene, Scene newScene)
    {
        Debug.Log("Scene change, scene is now " + SceneManager.GetActiveScene().name);
    }
}