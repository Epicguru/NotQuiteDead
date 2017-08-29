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

        byte error;
        int time = NetworkTransport.GetCurrentRTT(Player.Local.NetworkIdentity.connectionToServer.hostId, Player.Local.NetworkIdentity.connectionToServer.connectionId, out error);

        switch ((NetworkError)error)
        {
            case NetworkError.Ok:
                CommandProcessing.Log("RTT is " + time + "ms");
                return null;
            default:
                return "Error in NetworkTransport - " + (NetworkError)error;
        }


        
    }
}
