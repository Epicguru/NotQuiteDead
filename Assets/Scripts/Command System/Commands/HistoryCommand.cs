using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class HistoryCommand : Command
{
    public HistoryCommand()
    {
        Name = "history";
        parameters.Add(typeof(string));
    }

    public override string Execute(object[] args)
    {
        string action = (string)args[0];

        switch (action)
        {
            case "clear":

                CommandProcessing.lastCommands.Clear();
                CommandProcessing.Log("Cleared command history.");
                return null;
            case "list":
                foreach(string s in CommandProcessing.lastCommands)
                {
                    CommandProcessing.Log("   -" + s);
                }
                return null;
            case "count":
                CommandProcessing.Log("There are " + CommandProcessing.lastCommands.Count + "commands in history.");
                return null;
            default:
                return "Not a valid action! Use clear, list or count.";
        }
    }
}

