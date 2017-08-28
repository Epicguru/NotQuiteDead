using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class HelpCommand : Command
{
    public HelpCommand()
    {
        Name = "help";
    }

    private static StringBuilder builder = new StringBuilder();
    public override string Execute(object[] args)
    {
        foreach (Command c in CommandProcessing.GetCommands())
        {
            CommandProcessing.Log(c.Name + " - (" + c.GetParams(builder) + ")");
        }

        return null;
    }
}
