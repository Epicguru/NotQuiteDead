using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class GameTile : TileBase
{
    public string Prefab;
    public string Name;
    public bool Solid = true;
}
