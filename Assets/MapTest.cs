using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapTest : MonoBehaviour
{
    public Tilemap Foreground;
    public GameTile Tile;

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            Vector3Int pos = Foreground.WorldToCell(InputManager.GetMousePos());
            Foreground.SetTile(pos, Tile);
        }
    }
}