using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu Instance;

    public GameObject Menu;
    public bool MenuOpen;

    public void Awake()
    {
        Instance = this;
    }

    public void OnDestroy()
    {
        Instance = null;
    }

    public void Update()
	{
        Menu.SetActive(MenuOpen);
        if (InputManager.InputDown("Escape", MenuOpen))
        {
            if(Player.Local != null)
            {
                if (PlayerInventory.IsOpen)
                {
                    return;
                }
                else
                {
                    // Open or close menu.
                    MenuOpen = !MenuOpen;
                    if (MenuOpen)
                    {
                        UI.FlagOpen();
                    }
                    else
                    {
                        UI.FlagClosed();
                    }
                }
            }
        }
	}

    public void Button_Return()
    {
        MenuOpen = false;
        UI.FlagClosed();
    }

    public void Button_Save()
    {
        Debug.Log("Saving from pause menu...");
        World.Instance.Save();
        Button_Return();
    }

    public void Button_MainMenu()
    {
        Debug.Log("Quitting to main menu from pause menu, no save...");

        NetworkManager m = GameObject.FindObjectOfType<NetworkManager>();

        // Shut down all networking stuff.
        m.StopClient();
        m.StopHost();
        m.StopMatchMaker();
        Network.Disconnect();

        Destroy(m.gameObject);
        NetworkManager.Shutdown();
        NetworkTransport.Shutdown();

        SceneManager.LoadScene("Main Menu", LoadSceneMode.Single);
        //Invoke("SwapScene", 1f);
    }

    public void SwapScene()
    {        
        SceneManager.LoadScene("Main Menu", LoadSceneMode.Single);
    }
}