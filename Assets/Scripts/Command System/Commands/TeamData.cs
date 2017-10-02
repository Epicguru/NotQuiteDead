using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class TeamData : Command
{
    public TeamData()
    {
        Name = "team_data";
    }

    public override string Execute(object[] args)
    {
        CommandProcessing.Log(Teams.I.Data);
        return null;
    }
}
