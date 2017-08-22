using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[NetworkSettings(sendInterval = 0.05f)]
public class HandyRemoval : NetworkBehaviour {

    [SyncVar]
    private bool ShowHands = true;

    public Transform[] hands;

    public void CmdSetShowHands(bool flag)
    {
        ShowHands = flag;
    }

    public void Update()
    {
        foreach (Transform s in hands)
        {
            s.gameObject.SetActive(ShowHands);
        }
    }
}
