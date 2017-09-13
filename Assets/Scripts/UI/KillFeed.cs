using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class KillFeed : NetworkBehaviour {

    public static KillFeed Instance;

    [SyncVar]
    public string Feed = "";
    public Text Text;

    public void Start()
    {
        Instance = this;
        Text = GetComponent<Text>();
    }

    public void Update()
    {
        if(Text.text != Feed)
            Text.text = Feed;
    }

    [Server]
    public void ServerAddText(string text)
    {
        Feed += text + "\n";
    }

    [Server]
    public void ServerClear()
    {
        Feed = string.Empty;
    }
}
