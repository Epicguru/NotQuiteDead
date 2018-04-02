#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class MainMenuFunctions : MonoBehaviour
{
    public MainMenuTransitions T;
    public LoadingText LT;
    public MainMenuTransitionManager TM;
    public AsyncOperation AO;

    public void OpenCredits()
    {
        if (T.NotInMenu)
            return;

        TM.CurrentScreen = MainMenuScreen.CREDITS;
        T.NotInMenu = true;
    }

    public void LoadReality()
    {
        if (T.NotInMenu)
            return;

        TM.CurrentScreen = MainMenuScreen.REALITY_SELECT;
        T.NotInMenu = true;        
    }

    public void QuitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#endif
    }

    public void Update()
    {
        if (T.NotInMenu)
        {
            if(AO != null)
            {
                LT.Percentage = AO.progress * 100f;
            }
        }
    }
}