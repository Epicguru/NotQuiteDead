using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class ClearCommand : Command
{
    public ClearCommand()
    {
        Name = "clear";
    }

    public override string Execute(object[] args)
    {
        CommandProcessing.ClearLog();
        return null;
    }
}
