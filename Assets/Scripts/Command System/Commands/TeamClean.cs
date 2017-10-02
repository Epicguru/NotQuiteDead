using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class TeamClean : Command
{
    public TeamClean()
    {
        Name = "team_clean";
    }

    public override string Execute(object[] args)
    {
        if (!Player.Local.isServer)
            return "You are not the host! Cannot run team commands!";

        int removed = 0;

        List<string> bin = new List<string>();

        foreach(string t in Teams.I.GetTeamNames())
        {
            if (Teams.I.TeamEmpty(t))
            {
                bin.Add(t);
            }
        }

        foreach(string t in bin)
        {
            Teams.I.RemoveTeam(t);
            CommandProcessing.Log("Removed team '" + t + "'.");
            removed++;
        }

        CommandProcessing.Log("Removed a total of " + removed + " teams!");

        bin.Clear();

        return null;
    }
}
