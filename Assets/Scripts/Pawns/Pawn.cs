using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Health), typeof(ActiveObject), typeof(NetPosSync))]
[RequireComponent(typeof(PawnPathfinding))]
public class Pawn : NetworkBehaviour
{
    // Represents something that can move around the world map, using AI and pathfinding.

    // Static things.
    public static Dictionary<string, List<Pawn>> PawnMap = new Dictionary<string, List<Pawn>>();
    public static Dictionary<string, int> PawnCount = new Dictionary<string, int>();
    public static List<Pawn> AllPawns = new List<Pawn>();
    public static List<Pawn> EnemyPawns = new List<Pawn>();

    private static Dictionary<string, Pawn> loaded = new Dictionary<string, Pawn>();

    public string Prefab;
    public string Name;

    [Tooltip("Does it try to attack or harm the player?")]
    public bool IsEnemy = false;

    public int ID { get; private set; } // Not concurrent with client-server! Probably the same, but not necessarily.

    [Tooltip("A healthbar for this pawn to use, can be null.")]
    public HealthBar HealthBar;

    [HideInInspector]
    public Health Health;

    [HideInInspector]
    public ActiveObject AO;

    [HideInInspector]
    public PawnPathfinding Path;

    [HideInInspector]
    public NetPosSync NetPosSync;

    // What should a pawn do in an unloaded chunk?
    // Lets just load the chunks around pawns!

    public void Awake()
    {
        Health = GetComponent<Health>();
        AO = GetComponent<ActiveObject>();
        NetPosSync = GetComponent<NetPosSync>();
        Path = GetComponent<PawnPathfinding>();
    }

    public void Update()
    {
        if (HealthBar != null && Health != null)
        {
            HealthBar.MaxValue = Health.GetMaxHealth();
            HealthBar.CurrentValue = Health.GetHealth();
        }
    }

    public int GetTypeCount()
    {
        // How may spawned instances of this type of pwan are there?
        if(PawnMap.ContainsKey(Prefab))
            return PawnMap[Prefab].Count;
        return 0;
    }

    public void DestroyAllColliders()
    {
        foreach(Collider2D c in GetComponentsInChildren<Collider2D>())
        {
            Destroy(c);
        }
    }

    public List<Pawn> GetAllTypes()
    {
        // Gets a list of all spawned pawns of this type.
        if (PawnMap.ContainsKey(Prefab))
            return PawnMap[Prefab];
        return null;
    }

    public override void OnStartClient()
    {
        if (!PawnMap.ContainsKey(Prefab))
        {
            PawnMap.Add(Prefab, new List<Pawn>());
        }

        if(!PawnMap[Prefab].Contains(this))
            PawnMap[Prefab].Add(this);

        if (!PawnCount.ContainsKey(Prefab))
        {
            PawnCount.Add(Prefab, 1);
        }
        else
        {
            PawnCount[Prefab] += 1;
        }

        if (!AllPawns.Contains(this))
            AllPawns.Add(this);

        if (IsEnemy)
        {
            if (!EnemyPawns.Contains(this))
            {
                EnemyPawns.Add(this);
            }
        }

        ID = PawnCount[Prefab] - 1;
    }

    public void OnDestroy()
    {
        if (!PawnMap.ContainsKey(Prefab))
        {
            return;
        }

        if (PawnMap[Prefab].Contains(this))
            PawnMap[Prefab].Remove(this);

        if (AllPawns.Contains(this))
            AllPawns.Remove(this);

        if (IsEnemy)
        {
            if (EnemyPawns.Contains(this))
            {
                EnemyPawns.Remove(this);
            }
        }
    }

    public static void LoadAllPawns()
    {
        if (loaded == null)
            loaded = new Dictionary<string, Pawn>();

        Pawn[] x = Resources.LoadAll<Pawn>("Pawns/");
        foreach(Pawn p in x)
        {
            if(p.Prefab == null || p.Prefab.Trim() == "")
            {
                // Invalid pawn.
                continue;
            }

            loaded.Add(p.Prefab.Trim(), p);
            Debug.Log("Loaded pawn: " + p.Prefab);
        }
    }

    public static void RegisterPawns()
    {
        foreach(Pawn p in loaded.Values)
        {
            NetworkManager.singleton.spawnPrefabs.Add(p.gameObject);
        }
    }

    public static bool IsPawnLoaded(string prefab)
    {
        return loaded != null && loaded.ContainsKey(prefab);
    }

    public static Pawn GetPawn(string prefab)
    {
        if (!IsPawnLoaded(prefab))
        {
            Debug.LogError("No loaded pawn found for prefab name '" + prefab + "'.");
            return null;
        }

        return loaded[prefab];
    }

    [Server]
    public static Pawn SpawnPawn(string prefab, Vector3 location)
    {
        Pawn p = GetPawn(prefab);
        if(p == null)
        {
            return null;
        }

        GameObject g = Instantiate(p.gameObject);
        Pawn x = g.GetComponent<Pawn>();

        g.transform.position = location;

        NetworkServer.Spawn(g);

        return x;
    }

    public static void Dispose()
    {
        AllPawns.Clear();
        EnemyPawns.Clear();
        PawnCount.Clear();
        PawnMap.Clear();
        loaded.Clear();
    }
}