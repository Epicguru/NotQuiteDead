using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuFunctions : MonoBehaviour
{
    public void OpenCredits()
    {
        SceneManager.LoadScene("Credits", LoadSceneMode.Single);
    }

    public void LoadReality()
    {
        SceneManager.LoadScene("Setup V2", LoadSceneMode.Single);
    }
}