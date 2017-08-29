using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class TeleportCommand : Command
{
    public TeleportCommand()
    {
        Name = "tp";
        parameters.Add(typeof(float));
        parameters.Add(typeof(float));
    }

    public override string Execute(object[] args)
    {
        Player.Local.transform.position = new Vector3((float)args[0], (float)args[1]);
        CommandProcessing.Log("Teleported player to " + Player.Local.transform.position);
        return null;
    }
}
