using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class TeamSwap : Command
{
    public TeamSwap()
    {
        Name = "team_swap";
        parameters.Add(typeof(string));
        parameters.Add(typeof(string));
    }

    public override string Execute(object[] args)
    {
        if (!Player.Local.isServer)
            return "You are not the host! Cannot run team commands!";

        string player = args[0] as string;
        string newTeam = args[1] as string;

        if (string.IsNullOrEmpty(player.Trim()))
        {
            return "Name of player is empty!";
        }

        if (string.IsNullOrEmpty(newTeam.Trim()))
        {
            return "Name of team is empty!";
        }

        if (!Teams.I.PlayerInSystem(player))
        {
            return "Player not found in system!";
        }

        string oldTeam = Teams.I.GetTeamName(player);

        Teams.I.GetPlayer(player).Team = newTeam;
        CommandProcessing.Log("Moved player '" + player + "' from team '" + oldTeam + "' to new team '" + newTeam + "'.");

        return null;
    }
}
