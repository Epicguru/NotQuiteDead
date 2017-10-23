using Facepunch.Steamworks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class SteamManager : MonoBehaviour
{
    public uint AppID = 480;
    public TextureFormat format;
    public ServerList.Request ServerRequest;
    public Text Text;
    public UnityEngine.UI.Image Image;

    public void Awake()
    {
        // Do not destroy on scene change.
        DontDestroyOnLoad(gameObject);

        // Tell unity what we are running on; Windows, Mac, Console, Potato...
        Config.ForUnity(Application.platform.ToString());

        new Client(AppID);

        if(Client.Instance == null)
        {
            // Failed to 'start steam'. 
            Debug.LogError("Failed to create client instance!");
            return;
        }       

        ServerRequest = Client.Instance.ServerList.Internet();
        Client.Instance.Achievements.Refresh();
        Client.Instance.Friends.Refresh();
        Client.Instance.Screenshots.Trigger();

        Client.Instance.Overlay.OpenProfile(Client.Instance.SteamId);

        SteamFriend first = null;
        foreach(SteamFriend f in Client.Instance.Friends.AllFriends)
        {
            if (first == null || Random.value < 0.2f)
                first = f;

            Debug.Log(f.Name + ": " + (f.IsOnline ? "Online" : "Offline") + ", playing: " + f.CurrentAppId);
        }

        Debug.Log("Avatar for " + first.Name);
        Facepunch.Steamworks.Image image = Client.Instance.Friends.GetAvatar(Friends.AvatarSize.Small, first.Id);
        Debug.Log(image.Width + "x" + image.Height + ": " + image.IsLoaded);
        Texture2D tex = new Texture2D(image.Width, image.Height);
        for (int x = 0; x < image.Width; x++)
        {
            for (int y = 0; y < image.Height; y++)
            {
                var c = image.GetPixel(x, (image.Height - 1) - y);
                tex.SetPixel(x, y, new UnityEngine.Color((float)(c.r / 255f), (float)(c.g / 255f), (float)(c.b / 255f), 1f));
            }
        }

        tex.Apply();
        Image.material.mainTexture = tex;
    }

    public void Update()
    {
        if (Client.Instance != null)
        {
            Client.Instance.Update();
        }

        string t = "";
        t += "Username: " + Client.Instance.Username + "\n";
        t += "Install folder: " + Client.Instance.InstallFolder + "\n";
        t += "---QUERY---\n";
        t += "Status: " + (ServerRequest.Finished ? "Finished" : "Querying") + "\n";
        t += "Responded: " + ServerRequest.Responded.Count + "\n";
        t += "Unresponsive: " + ServerRequest.Unresponsive.Count + "\n";

        for (int i = 0; i < ServerRequest.Responded.Count; i++)
        {
            t += "Server #" + i + ": " + ServerRequest.Responded[i].AddressString + " - " + ServerRequest.Responded[i].Name + " - Online:" + ServerRequest.Responded[i].Players + "\n";
        }

        Text.text = t;
    }

    private void OnDestroy()
    {
        if (Client.Instance != null)
        {
            Debug.Log("Exiting steam client.");
            Client.Instance.Dispose();
        }

        if(Server.Instance != null)
            Server.Instance.Dispose();
    }
}