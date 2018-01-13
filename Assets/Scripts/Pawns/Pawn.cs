using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Health), typeof(ActiveObject), typeof(NetPositionSync))]
[RequireComponent(typeof(PawnPathfinding))]
public class Pawn : NetworkBehaviour
{
    // Represents something that can move around the world map, using AI and pathfinding.

    // Static things.
    public static Dictionary<string, List<Pawn>> PawnMap = new Dictionary<string, List<Pawn>>();
    public static Dictionary<string, int> PawnCount = new Dictionary<string, int>();

    public string Prefab;
    public string Name;
    public int ID { get; private set; } // Not concurrent with client-server! Probably the same, but not necessarily.

    [HideInInspector]
    public Health Health;

    [HideInInspector]
    public ActiveObject AO;

    [HideInInspector]
    public NetPositionSync NetPositionSync;

    // What should a pawn do in an unloaded chunk?
    // Lets just load the chunks around pawns!

    public void Awake()
    {
        Health = GetComponent<Health>();
        AO = GetComponent<ActiveObject>();
        NetPositionSync = GetComponent<NetPositionSync>();
    }

    public int GetTypeCount()
    {
        // How may spawned instances of this type of pwan are there?
        if(PawnMap.ContainsKey(Prefab))
            return PawnMap[Prefab].Count;
        return 0;
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
    }
}