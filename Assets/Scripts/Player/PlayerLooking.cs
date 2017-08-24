using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerLooking : NetworkBehaviour {

    public bool Looking;

    private PlayerDirection direction;
    private PlayerHeadLook look;

    private void Start()
    {
        direction = GetComponent<PlayerDirection>();
        look = GetComponent<PlayerHeadLook>();
    }

    public void Update()
    {
        direction.TrackMouse = Looking;
        look.Active = Looking;
    }
}
