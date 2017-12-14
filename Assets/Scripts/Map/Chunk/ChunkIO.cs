using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

public static class ChunkIO
{
    public const string NULL_ERROR = "NULL,";

    public static string MakeString(BaseTile[][] tiles, int chunkX, int chunkY, int chunkSize)
    {
        if (tiles == null)
        {
            Debug.LogError("Tiles input is null.");
            return null;
        }

        StringBuilder str = new StringBuilder();

        for (int x = chunkX * chunkSize; x < chunkX * chunkSize + chunkSize; x++)
        {
            for (int y = chunkY * chunkSize; y < chunkY * chunkSize + chunkSize; y++)
            {
                BaseTile tile = tiles[x][y];
                if(tile == null)
                {
                    str.Append(NULL_ERROR);
                }
                else
                {
                    str.Append(tiles[x][y].Prefab.Trim() + ",");
                }
            }
        }

        return str.ToString();
    }

    private static BaseTile[][] chunk;
    public static BaseTile[][] MakeChunk(string str, int chunkSize, UnityEvent<string> error = null)
    {
        if(chunk == null || chunk.Length != chunkSize)
        {
            chunk = new BaseTile[chunkSize][];
        }

        string[] parts = str.Split(',');

        for (int x = 0; x < chunkSize; x++)
        {
            for (int y = 0; y < chunkSize; y++)
            {
                if(chunk[x] == null || chunk[x].Length != chunkSize)
                {
                    chunk[x] = new BaseTile[chunkSize];
                }

                int index = x + chunkSize * y;

                string prefab = parts[index].Trim();

                if (prefab == NULL_ERROR)
                    continue;

                if (BaseTile.ContainsTile(prefab))
                {
                    chunk[x][y] = BaseTile.GetTile(prefab);
                }
                else
                {
                    if(error != null)
                        error.Invoke("Tile '" + prefab + "' not found @ chunk " + x + ", " + y + ".");
                }
            }
        }

        return chunk;
    }
}