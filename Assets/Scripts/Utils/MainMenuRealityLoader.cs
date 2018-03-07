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
	}

    public void SceneChange()
    {
        Debug.Log("Scene change, scene is now " + SceneManager.GetActiveScene().name);
    }
	
	public void Update ()
	{
		
	}
}