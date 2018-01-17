using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;

public class MenuNet : MonoBehaviour {

    public NetworkManager Net;

    public void HostNewGame()
    {
        Net.StartHost();
    }

    public void JoinLocalhost()
    {
        Net.StartClient();
    }
}
