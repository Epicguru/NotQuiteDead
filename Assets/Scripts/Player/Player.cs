using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class Player : NetworkBehaviour
{
    [SyncVar(hook = "NewName")]
    public string Name = ERROR_NAME;
    [SyncVar(hook = "NewTeam")]
    public string Team = ERROR_TEAM;

    public const string ERROR_NAME = "ERROR_NAME"; // We can safly assume notbody will ever have this name!
    public const string ERROR_TEAM = "ERROR_TEAM"; // We can safly assume notbody will ever have this name!

    public PlayerAnimation Animation;
    public PlayerDirection Direction;
    public PlayerHeadLook HeadLook;
    public PlayerHolding Holding;
    public PlayerLooking Looking;
    public PlayerMovement Movement;
    public PlayerEquipment Equipment;
    public PlayerNetUtils NetUtils;
    public HandyRemoval HandyRemoval;
    public NetworkIdentity NetworkIdentity;
    public Health Health;
    public QuickSlot QuickSlot;
    public SpriteLighting Lighting;
    public BodyGearStats GearStats;
    public BodyGear[] BodyGear;
    public Dictionary<string, BodyGear> GearMap = new Dictionary<string, BodyGear>();
    public Player _Player;

    public static Player Local;
    public static List<Player> AllPlayers = new List<Player>();

    public void RegisterPlayer()
    {
        AllPlayers.Add(this);
        if (isServer)
        {
            string name = NameGen.GenName();
            while (PlayerHasName(name))
            {
                name = NameGen.GenName();
            }
            // Give name and team
            Team = AllPlayers.Count % 2 == 0 ? "Blue" : "Red";
            Name = name;

        }
        UpdateTeams(null, null); // New state
    }

    public void UpdateTeams(string oldName, string oldTeam)
    {
        if (Team == oldTeam && Name == oldName)
            return;

        //Debug.Log("Call; oldName: '" + oldName + "' Name: '" + Name + "' oldTeam: '" + oldTeam + "' Team: " + Team);

        // Updates team and player data for the local Teams system.
        Teams t = Teams.I;
            
        if(Name != oldName)
        {
            if (string.IsNullOrEmpty(oldName))
            {
                // We did not have an old name...
                // Check to see if we are on the system (we should not be!) and add the player
                if (!t.PlayerInSystem(Name))
                {
                    t.EnsureTeam(Team);
                    t.AddPlayer(this, Team);
                    Debug.Log("Added player to system. Name: " + Name + ", Team: " + Team);
                }
            }
            else
            {
                // We have an old name, lets change names!
                if (t.PlayerInSystem(oldName))
                {
                    t.ChangePlayerName(oldName, Name);
                    Debug.Log("Changed player name from '" + oldName + "' to '" + Name + "'");
                }
                else
                {
                    t.EnsureTeam(Team);
                    t.AddPlayer(this, Team);
                }
            }
        }

        if(Team != oldTeam)
        {
            if (string.IsNullOrEmpty(oldTeam))
            {
                // Means that this must be first time commit, meaning that we have already been added to team!
            }
            else
            {
                // Old team is not null, lets swap teams!
                if (t.PlayerInSystem(Name))
                {
                    t.EnsureTeam(Team);
                    t.ChangePlayerTeam(Name, Team);
                    Debug.Log("Changed team of player '" + Name + "' from team '" + oldTeam + "' to '" + Team + "'");
                }
                else
                {
                    t.EnsureTeam(Team);
                    t.AddPlayer(this, Team);
                    Debug.Log("Added player WHILE changing team: '" + Name + "' from team '" + oldTeam + "' to '" + Team + "'");
                }
                
            }
        }
    }

    public void Start()
    {
        RegisterPlayer();
        RegisterGear();

        if (isLocalPlayer)
        {            
            Local = this;
            Local._Player = this;
            Camera.main.GetComponent<CameraFollow>().Target = transform;

            Health.UponDeath += UponDeath;
        }
        else
        {
            Debug.Log("Player '" + Name + "' of team '" + Team + "'");
        }

        foreach (SpriteRenderer r in GetComponentsInChildren<SpriteRenderer>())
        {
            r.sortingLayerName = "Player";
        }

        NetUtils.CmdSetGear("Chest", "Space Suit", new ItemData());
        NetUtils.CmdSetGear("Head", "SSH", new ItemData());
    }

    public void OnDestroy()
    {
        AllPlayers.Remove(this);
        Teams.I.RemovePlayer(this.Name);
    }

    public Player GetPlayer(string name)
    {
        foreach (Player p in AllPlayers)
        {
            if (p.Name.Trim().ToLower() == name.Trim().ToLower())
                return p;
        }

        return null;
    }

    public bool PlayerHasName(string name)
    {
        foreach (Player p in AllPlayers)
        {
            if (p.Name.Trim().ToLower() == name.Trim().ToLower())
                return true;
        }
        return false;
    }

    public void RegisterGear()
    {
        foreach(BodyGear gear in BodyGear)
        {
            Debug.Log("Mapped gear slot: " + gear.Name);
            GearMap.Add(gear.Name, gear);
        }
    }

    public void Update()
    {
        gameObject.name = Name;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if(GearMap["Chest"].GetGearItem() != null)
            {
                NetUtils.CmdSetGear("Chest", null, null);
            }
            else
            {
                NetUtils.CmdSetGear("Chest", "Space Suit", new ItemData());
            }
        }
    }

    private void UponDeath()
    {
        // 'Respawn'
        Health.CmdHeal(Health.GetMaxHealth());
        transform.position = Vector3.zero;
        Debug.Log("Player died:");
        string report = Health.GetDamageReport("killed");
        Debug.Log("Local player was " + report);

        // Add to kill feed.
        Player.Local.NetUtils.CmdAddKill(Health.GetKiller(), this.Name, Health.IsActiveKiller() ? Health.GetKillerItem() : null);
    }

    /// <summary>
    /// Requests for authority of this game object. The object must be mamaged (spawned) by the server.
    /// </summary>
    public void RequestAuthority(GameObject obj)
    {
        if (!isLocalPlayer)
        {
            Debug.LogError("Somebody tried to request authority from a player that is not theirs.");
            return;
        }
        Debug.Log("Requesting authority for " + obj.name);
        NetUtils.CmdReqAuth(obj);
    }

    public void NewName(string newName)
    {
        string old = Name;
        Name = newName;
        UpdateTeams(old, Team);
    }

    public void NewTeam(string newTeam)
    {
        string old = Team;
        Team = newTeam;
        UpdateTeams(Name, old);
    }

    public void ExplosionAt(Vector2 position)
    {
        if (!isLocalPlayer)
            return;

        const float MAX_DISTANCE = 40f;
        const float MAX_SHAKE = 4f;

        float distance = Vector2.Distance(position, transform.position);

        if (distance > MAX_DISTANCE)
            return;

        float nomalized = 1f - Mathf.Clamp(distance / MAX_DISTANCE, 0f, 1f);

        Camera.main.GetComponent<CameraShake>().ShakeCamera(nomalized * MAX_SHAKE);
    }
}