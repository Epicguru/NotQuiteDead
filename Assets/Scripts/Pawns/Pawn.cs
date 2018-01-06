using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Health))]
public class Pawn : NetworkBehaviour
{
    // Represents something that can move around the world map, using AI and pathfinding.

    public string Prefab;
    public string Name;

    [HideInInspector]
    public Health Health;

    // What should a pawn do in an unloaded chunk?
    // Lets just load the chunks around pawns!


}