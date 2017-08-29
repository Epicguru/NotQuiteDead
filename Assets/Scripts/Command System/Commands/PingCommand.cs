using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.Networking;

public class PingCommand : Command
{
    public PingCommand()
    {
        Name = "ping";
    }

    public override string Execute(object[] args)
    {
        int time = NetworkManager.singleton.client.GetRTT();

        CommandProcessing.Log("Client RTT is " + time + "ms");

        return null;        
    }
}
