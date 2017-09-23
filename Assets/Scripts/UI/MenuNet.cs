using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;

public class MenuNet : MonoBehaviour {

    public NetworkLobbyManager Net;

    public void HostNewGame()
    {
        Net.StartHost();
        Net.SendReturnToLobby();
    }

    public void JoinLocalhost()
    {
        Net.StartClient();
    }
}
