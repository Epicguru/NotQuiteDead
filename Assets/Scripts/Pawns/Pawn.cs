using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Health))]
public class Pawn : NetworkBehaviour
{
    public string Prefab;
    public string Name;

    [HideInInspector]
    public Health Health;

    // What should a pawn do in an unloaded chunk?
    // Lets just load the chunks around pawns!


}