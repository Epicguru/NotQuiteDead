using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Health), typeof(ActiveObject), typeof(NetPositionSync))]
public class Pawn : NetworkBehaviour
{
    // Represents something that can move around the world map, using AI and pathfinding.

    public string Prefab;
    public string Name;

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
}