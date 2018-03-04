using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class MainMenuFunctions : MonoBehaviour
{
    public MainMenuTransitions T;
    public LoadingText LT;
    public AsyncOperation AO;

    public void OpenCredits()
    {
        if (T.IsLoading)
            return;
        SceneManager.LoadScene("Credits", LoadSceneMode.Single);
    }

    public void LoadReality()
    {
        T.IsLoading = true;
        LT.RealityName = "Devland";
        LT.RealityDay = 123;
        AsyncOperation ao = SceneManager.LoadSceneAsync("Setup V2", LoadSceneMode.Single);
        ao.completed += LoadRealitySceneLoaded;
        AO = ao;
    }

    private void LoadRealitySceneLoaded(AsyncOperation ao)
    {
        Debug.Log("Scene loaded, starting host...");
        GameObject.Find("Network Manager").GetComponent<NetworkManager>().StartHost();
        AO = null;
    }

    public void Update()
    {
        if (T.IsLoading)
        {
            if(AO != null)
            {
                LT.Percentage = AO.progress * 100f;
                Debug.Log(AO.progress);
            }
        }
    }
}