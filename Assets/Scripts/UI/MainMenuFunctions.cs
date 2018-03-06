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

        //TM.CurrentScreen = MainMenuScreen.LOADING;
        //T.NotInMenu = true;
        //LT.RealityName = "Devland";
        //LT.RealityDay = 123;
        //AsyncOperation ao = SceneManager.LoadSceneAsync("Setup V2", LoadSceneMode.Single);
        //ao.completed += LoadRealitySceneLoaded;
        //AO = ao;
    }

    private void LoadRealitySceneLoaded(AsyncOperation ao)
    {
        Debug.Log("Scene loaded, starting host...");
        GameObject.Find("Network Manager").GetComponent<NetworkManager>().StartHost();
        AO = null;
    }

    public void Update()
    {
        if (T.NotInMenu)
        {
            if(AO != null)
            {
                LT.Percentage = AO.progress * 100f;
                Debug.Log(AO.progress);
            }
        }
    }
}