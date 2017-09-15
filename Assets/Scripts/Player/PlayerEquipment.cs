using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerEquipment : NetworkBehaviour
{
    /*
     * The player can have multiple items and gear equipped on them at one time.
     * This includes melee and range weapons, vanity and armour gear (TODO) and things like that.
     */

    // That means:
    // 1. We need to hand objects to the 'Holding' class to be held, but:
    // 2. We need to manage stored items ourselves, including equipped gear.

    // Ideas:
    // 1. Have the items be quick access slots.
    // 2. Just have this be a basic visual aid for equipped items. <---

    public void Update()
    {
        if (Player.Local == null)
            return;

        if(Player.Local.Holding.Item == null)
        {

        }
    }
}
