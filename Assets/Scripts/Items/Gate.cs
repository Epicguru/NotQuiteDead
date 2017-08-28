using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Placeable))]
public class Gate : NetworkBehaviour
{
    [SyncVar]
    public bool IsOpen;
    public Animator Animator;
    public string OpenBool = "Open";

    public void Update()
    {
        Animator.SetBool(OpenBool, IsOpen);
    }

    [Command]
    public void CmdSetOpen(bool open)
    {
        IsOpen = open;
    }
}
