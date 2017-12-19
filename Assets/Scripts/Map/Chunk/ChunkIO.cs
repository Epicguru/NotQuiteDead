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
    public const char COMMA = ',';
    public const char NULL_ERROR = '?';

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
                    bool last = (x == (chunkX * chunkSize + chunkSize) - 1) && (y == (chunkY * chunkSize + chunkSize) - 1);
                    if (!last)
                        str.Append(COMMA);
                }
                else
                {
                    str.Append(tiles[x][y].Prefab.Trim());
                    bool last = (x == (chunkX * chunkSize + chunkSize) - 1) && (y == (chunkY * chunkSize + chunkSize) - 1);
                    if (!last)
                        str.Append(COMMA);
                }
            }
        }

        return str.ToString();
    }

    public static bool IsChunkSaved(string reality, string layer, int chunkX, int chunkY)
    {
        string path = GetPathForChunk(reality, layer, chunkX, chunkY);

        return IsChunkSaved(path);
    }

    public static bool IsChunkSaved(string path)
    {
        return File.Exists(path);
    }

    public static string GetPathForChunk(string reality, string layer, int chunkX, int chunkY)
    {
        return OutputUtils.RealitySaveDirectory + reality + OutputUtils.RealityLayersDirectory + layer + OutputUtils.RealityChunksDirectory + GetFileName(chunkX, chunkY); ;
    }

    public static void LoadChunk(string reality, string layer, int chunkX, int chunkY, int chunkSize, UnityAction<object[]> done, UnityAction<string> error = null)
    {
        Thread thread = new Thread(() => 
        {
            string path = GetPathForChunk(reality, layer, chunkX, chunkY);

            if (!IsChunkSaved(path))
            {
                if (error != null)
                    error.Invoke("A file for that chunk could not be found! (" + GetPathForChunk(reality, layer, chunkX, chunkY));
                return;
            }

            // Read all data
            string text = File.ReadAllText(path);
            text = text.Trim();

            if (string.IsNullOrEmpty(text))
            {
                if (error != null)
                    error.Invoke("File existed, but text was null or empty!");

                Threader.Instance.PostAction(done, false, chunkX, chunkY, null, layer, reality);

                return;
            }

            Debug.Log("Loaded chunk @ " + chunkX + ", " + chunkY);


            BaseTile[][] tiles = MakeChunk(text, chunkSize, error);

            if (tiles == null)
            {
                if (error != null)
                {
                    error.Invoke("Tile 2D array creation failed.");
                    return;
                }

                Threader.Instance.PostAction(done, false, chunkX, chunkY, null, layer, reality);

            }

            Threader.Instance.PostAction(done, true, chunkX, chunkY, tiles, layer, reality);
        });
        thread.Start();
    }

    private static BaseTile[][] chunk;
    public static BaseTile[][] MakeChunk(string str, int chunkSize, UnityAction<string> error = null)
    {
        if(chunk == null || chunk.Length != chunkSize)
        {
            chunk = new BaseTile[chunkSize][];
        }

        string[] parts = str.Split(',');

        if(parts.Length != chunkSize * chunkSize)
        {
            if (error != null)
                error.Invoke("Incorrect number of tiles for chunk: found " + parts.Length + " when " + chunkSize * chunkSize + " were expected.");
            return null;
        }

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

                if (prefab == NULL_ERROR.ToString())
                    continue;

                if (BaseTile.ContainsTile(prefab))
                {
                    chunk[x][y] = BaseTile.GetTile(prefab);
                }
                else
                {
                    if(error != null)
                        error.Invoke("Tile '" + prefab + "' not found @ local position " + x + ", " + y + ".");
                }
            }
        }

        return chunk;
    }
}