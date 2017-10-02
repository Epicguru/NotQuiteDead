using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teams : MonoBehaviour
{
    [TextArea(20, 50)]
    public string Data;

    public static Teams I;

    private Dictionary<string, List<Player>> ActiveTeams = new Dictionary<string, List<Player>>();
    private Dictionary<string, Player> PlayerNames = new Dictionary<string, Player>();
    private Dictionary<string, string> PlayerNameToTeamName = new Dictionary<string, string>();

    public void AddPlayer(Player player, string team)
    {
        // Note that the current system assumes that a players name never changes...
        // This adds to the system and to the team!

        if(!TeamExists(team))
        {
            Debug.LogError("Team does not exist, player cannot be added! (player:" + player.Name + ", team:" + team + ").");
            return;
        }
        if (PlayerInSystem(player.Name))
        {
            Debug.LogError("Player " + player.Name + " is already in the team system! They cannot be added twice, and duplicate names cannot be added!");
            return;
        }

        PlayerNames.Add(player.Name, player);
        PlayerNameToTeamName.Add(player.Name, team);
        ActiveTeams[team].Add(player);
    }

    public void RemovePlayer(string playerName)
    {
        if (!PlayerInSystem(playerName))
        {
            Debug.LogError("The player " + playerName + " is not in the player system! Cannot be removed!");
            return;
        }
        Player player = PlayerNames[playerName];
        if(player == null)
        {
            Debug.LogError("The player is null, object may have been destroyed. Fatal error!");
            return;
        }

        // Remove from team
        ActiveTeams[PlayerNameToTeamName[playerName]].Remove(player); // Remove from team...
        PlayerNameToTeamName.Remove(playerName); // Remove player-team link.
        PlayerNames.Remove(playerName); // Remove name-player link.
    }

    public void ChangePlayerTeam(string player, string newTeam)
    {
        if (!TeamExists(newTeam))
        {
            Debug.LogError("The new team " + newTeam + " does not exist, create it before adding a player to it!");
            return;
        }

        if (!PlayerInSystem(player))
        {
            Debug.LogError("The player " + player + " is not in the team system, cannot change team! (duh)");
            return;
        }

        ActiveTeams[PlayerNameToTeamName[player]].Remove(PlayerNames[player]); // Remove from old team...
        PlayerNameToTeamName[player] = newTeam; // Change association, then
        ActiveTeams[newTeam].Add(PlayerNames[player]); // Add to new team!
    }

    public void ChangePlayerName(string oldPlayerName, string newName)
    {
        if (!PlayerInSystem(oldPlayerName))
        {
            Debug.LogError("Player " + oldPlayerName + " was not part of the team system...");
            return;
        }
        if (PlayerInSystem(newName))
        {
            Debug.LogError("The player " + oldPlayerName + " cannot change name in the team system because that name is already registered to a player!");
            return;
        }

        Player player = PlayerNames[oldPlayerName];

        // Remove from name-player link
        PlayerNames.Remove(oldPlayerName);
        // Add new link (sort of replace)
        PlayerNames.Add(newName, player);

        // Change player-team link
        string team = PlayerNameToTeamName[oldPlayerName];
        PlayerNameToTeamName.Remove(oldPlayerName);
        PlayerNameToTeamName.Add(newName, team);

        // Team list does not need to change, because we assume that the name on the player object has been changed, and that
        // the team of the player has not been changed.
    }

    public bool TeamExists(string team)
    {
        return ActiveTeams.ContainsKey(team);
    }

    public bool TeamEmpty(string team)
    {
        if (!TeamExists(team))
            return true;
        return ActiveTeams[team].Count == 0;
    }

    public List<Player> GetTeam(string team)
    {
        if (!TeamExists(team))
        {
            Debug.LogError("Team " + team + " does not exist!");
            return null;
        }

        return ActiveTeams[team];
    }

    public Dictionary<string, Player>.ValueCollection GetAllPlayers()
    {
        return PlayerNames.Values;
    }

    public Player GetPlayer(string name)
    {
        if (!PlayerInSystem(name))
        {
            Debug.LogError("Player " + name + "is not in system!");
            return null;
        }

        return PlayerNames[name];
    }

    public bool PlayersInSameTeam(string playerA, string playerB)
    {
        if (!PlayerInSystem(playerA))
        {
            Debug.LogError("Player " + playerA + " (first arg) is not part of the system! Cannot compare teams.");
            return false;
        }
        if (!PlayerInSystem(playerB))
        {
            Debug.LogError("Player " + playerB + " (second arg) is not part of the system! Cannot compare teams.");
            return false;
        }

        string teamA = PlayerNameToTeamName[playerA];
        string teamB = PlayerNameToTeamName[playerB];
        return teamA == teamB;
    }

    public bool PlayerInTeam(string player, string team)
    {
        if (!PlayerInSystem(player))
        {
            Debug.LogError("Player " + player + " is not part of the system!");
            return false;
        }
        return PlayerNameToTeamName[player] == team;
    }

    public bool PlayerInSystem(string playerName)
    {
        return PlayerNames.ContainsKey(playerName);
    }

    public string GetTeamName(Player player)
    {
        return GetTeamName(player.Name);
    }

    public string GetTeamName(string player)
    {
        return PlayerNameToTeamName[player];
    }

    public void RemoveTeam(string teamName)
    {
        if (!TeamExists(teamName))
        {
            Debug.LogError("The team " + teamName + " does not exist in the system, cannot be removed! (duh)");
            return;
        }

        if (!TeamEmpty(teamName))
        {
            Debug.LogError("Team cannot be removed until the team is empty, please remove players from the system or change their teams!");
            return;
        }

        ActiveTeams.Remove(teamName);
    }

    public void AddTeam(string newTeamName)
    {
        if (string.IsNullOrEmpty(newTeamName.Trim()))
        {
            Debug.LogError("Team name cannot be null or empty, when trimmed!");
            return;
        }

        if (TeamExists(newTeamName))
        {
            Debug.LogError("The team " + newTeamName + " already exist in the system, cannot be added!");
            return;
        }

        ActiveTeams.Add(newTeamName, new List<Player>());
    }

    public void EnsureTeam(string team)
    {
        if (string.IsNullOrEmpty(team))
        {
            Debug.LogError("The team name to ensure is null or empty!");
            return;
        }
        if (!TeamExists(team))
            AddTeam(team);
    }

    public Dictionary<string, List<Player>>.KeyCollection GetTeamNames()
    {
        return ActiveTeams.Keys;
    }

    public void ForceWipe()
    {
        // Removes all data from the class, like resetting it. Ignores all rules.
        ActiveTeams.Clear();
        PlayerNameToTeamName.Clear();
        PlayerNames.Clear();
    }

    public void Awake()
    {
        I = this;
    }

    public void Update()
    {
        Data = "";
        Data += GetAllPlayers().Count + " players in sys, currently " + Player.AllPlayers.Count + " player GOs.\n";
        Data += ActiveTeams.Count + " teams:\n\n";
        Data += ("Pretty:\n");
        foreach(string s in GetTeamNames())
        {
            Data += "* Team '" + s + "'\n";
            foreach(Player p in GetTeam(s))
            {
                Data += "   -" + p.Name + "\n";
            }
        }
        Data += ("Raw:\n");
        foreach(string s in PlayerNameToTeamName.Keys)
        {
            Data += "   -" + s + " of team " + PlayerNameToTeamName[s] + "\n";
        }
    }
}
