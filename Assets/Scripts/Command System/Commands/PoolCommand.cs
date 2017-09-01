using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class PoolCommand : Command
{
    public PoolCommand()
    {
        Name = "pool";
        parameters.Add(typeof(string));
    }

    public override string Execute(object[] args)
    {
        string action = (string)args[0];

        switch(action){
            case "clear":

                int total = ObjectPool.GetObjectCount();

                ObjectPool.Clear();
                CommandProcessing.Log("Removing all " + total + " idle pooled objects.");

                return null;
            case "stats":

                CommandProcessing.Log(ObjectPool.GetStats());

                return null;
            default:

                return "Invalid action. Valid action are clear, stats.";
        }
    }
}
