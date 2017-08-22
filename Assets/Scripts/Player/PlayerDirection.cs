using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[NetworkSettings(sendInterval = 0.05f)]
public class PlayerDirection : NetworkBehaviour {

    public Transform PlayerGraphics;
    public bool TrackMouse;

    private Vector3 scale = new Vector3();

    [SyncVar]
    public bool Right;

    public void Update()
    {
        if (isLocalPlayer)
        {
            if (TrackMouse)
                LookAtMouse();
            CmdSetDirection(Right);
        }

        scale.Set(Right ? 1 : -1, 1, 1);
        PlayerGraphics.localScale = scale;
    }

    [Command]
    private void CmdSetDirection(bool right)
    {
        this.Right = right;
    }

    private void LookAtMouse()
    {
        // Turns the player left or right in order to look at mouse, such as when aiming.

        if(InputManager.GetMousePos().x >= transform.position.x)
        {
            // Look right.
            Right = true;
        }
        else
        {
            // Look left.
            Right = false;
        }

    }
}
