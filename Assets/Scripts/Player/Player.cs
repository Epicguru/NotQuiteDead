﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
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
    public BodyGearStats GearStats;
    public ActiveObject AO;
    public BuildingInventory BuildingInventory;
    public PlayerBuilding Building;
    public BodyGear[] BodyGear;
    public Dictionary<string, BodyGear> GearMap = new Dictionary<string, BodyGear>();
    public Player _Player;

    public bool RequestGear = true;

    public static Player Local;
    public static List<Player> AllPlayers = new List<Player>();

    public void RegisterPlayer()
    {
        AllPlayers.Add(this);
        if (isServer)
        {
            int i = 0;
            string name = "Player " + i;
            while (PlayerHasName(name))
            {
                name = "Player " + i;
                i++;
            }
            // Give name and team
            Team = AllPlayers.Count % 2 == 0 ? "Blue" : "Red";
            Name = name;

        }
        UpdateTeams(null, null); // New state

        if (isLocalPlayer)
        {
            NetworkManager.singleton.client.RegisterHandler((short)MessageTypes.SEND_CHUNK_DATA, ReceivedChunkData);
            NetworkManager.singleton.client.RegisterHandler((short)MessageTypes.SEND_TILE_CHANGE, ReceivedTileChange);
        }
    }

    [Client]
    private void ReceivedChunkData(NetworkMessage message)
    {
        // Called when receiving a chunk from the server.
        Msg_SendChunk msg = message.ReadMessage<Msg_SendChunk>();

        World.Instance.TileMap.GetLayer(msg.Layer).RecievedNetChunk(msg.Data, msg.ChunkX, msg.ChunkY);
    }

    [Client]
    private void ReceivedTileChange(NetworkMessage message)
    {
        // Called when a tile has changed in the world.
        Msg_SendTile data = message.ReadMessage<Msg_SendTile>();

        if (string.IsNullOrEmpty(data.Prefab))
        {
            data.Prefab = null;
            // Seems pointless, but unet refuses to have null strings! Really annoying when null is a perfectly valid value!
        }

        World.Instance.TileMap.GetLayer(data.Layer).RecievedTileChange(data);
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
            UI.Reset();
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
            if (r.gameObject.layer != 9)
                r.sortingLayerName = "Player";
        }

        if (isLocalPlayer)
        {
            // Apply no gear, TODO load from file if necessary.
        }
        else if(!isServer)
        {
            // Request updated gear
            RequestGear = true;
        }
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
        if (RequestGear)
        {
            // Request the gear for this player.    
            if(!isServer)
                Player.Local.NetUtils.CmdRequestGear(this.gameObject);
            RequestGear = false;
        }

        gameObject.name = Name;

        DebugText.Log("[" + Name + " - " + Team + "] @ " + transform.position.ToString("F0"), Color.cyan);

        // If this is the local player, load the chunks around them.
        if (isLocalPlayer)
        {
            RectInt bounds = World.Instance.TileMap.GetCameraChunkBounds();
            int chunkSize = World.Instance.TileMap.ChunkSize;
            int x = bounds.xMin / chunkSize;
            int y = bounds.yMin / chunkSize;
            int endX = bounds.xMax / chunkSize;
            int endY = bounds.yMax / chunkSize;

            AO.CancelAllRequests();

            for (int X = x; X < endX; X++)
            {
                for (int Y = y; Y < endY; Y++)
                {
                    AO.RequestChunk(X, Y);                    
                }
            }
        }
    }

    private void UponDeath()
    {
        if (!isLocalPlayer)
            return;

        // Respawn:

        // Heal to max health.
        Health.CmdHeal(Health.GetMaxHealth());

        // Go to world spawn point...
        transform.position = World.Instance.GetRandomSpawnPoint();

        // Add to kill feed.
        NetUtils.CmdAddKill(Health.GetKiller(), this.Name, Health.IsActiveKiller() ? Health.GetKillerItem() : null);
        string report = Health.GetDamageReport("killed");
        Debug.Log("Local player was " + report);
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