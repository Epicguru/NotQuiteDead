using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;

public static class ChunkIO
{
    public const string NULL_ERROR = "?,";

    public static string GetFileName(int chunkX, int chunkY)
    {
        return chunkX + ", " + chunkY + ".chunk";
    }

    public static void SaveChunk(string reality, string layer, BaseTile[][] tiles, int chunkX, int chunkY, int chunkSize, UnityAction<object[]> done)
    {
        string path = OutputUtils.RealitySaveDirectory + reality + OutputUtils.RealityLayersDirectory + layer + OutputUtils.RealityChunksDirectory + GetFileName(chunkX, chunkY);

        Thread thread = new Thread(() => 
        {
            string toSave = MakeString(tiles, chunkX, chunkY, chunkSize);

            string dir = Path.GetDirectoryName(path);

            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            File.WriteAllText(path, toSave);

            Thread.Sleep(2000);

            Debug.Log("Saved chunk @ " + chunkX + ", " + chunkY);

            Threader.Instance.PostAction(done, chunkX, chunkY, layer, reality, path);                
        });

        thread.Start();
        // TODO add a unity event system, with params, that executes in main thread.
    }

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
    public static BaseTile[][] MakeChunk(string str, int chunkSize, UnityAction<string> error = null)
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