﻿using System;
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

            // Run RLE
            float efficiency;
            toSave = RLE(toSave, out efficiency);

            //Debug.Log("Chunk RLE efficiency: " + (int)(efficiency * 100f) + "%");

            string dir = Path.GetDirectoryName(path);

            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            File.WriteAllText(path, toSave);

            //Debug.Log("Saved chunk @ " + chunkX + ", " + chunkY);

            Threader.Instance.PostAction(done, chunkX, chunkY, layer, reality, path);                
        });

        thread.Start();
        // TODO add a unity event system, with params, that executes in main thread.
    }

    public static void GetChunkForNet_Loaded(GameObject targetPlayer, BaseTile[][] tiles, int chunkX, int chunkY, int chunkSize, UnityAction<object[]> done, bool rle = true)
    {
        Thread thread = new Thread(() => 
        {
            string basic = MakeString(tiles, chunkX, chunkY, chunkSize);

            if (basic == null)
            {
                Debug.LogError("Chunk string could not be made when requested for Net!");
                Threader.Instance.PostAction(done, false, null, chunkX, chunkY);
                return;
            }

            string final;

            if (rle)
            {
                float eff;
                final = RLE(basic, out eff);
            }
            else
            {
                final = basic;
            }

            Threader.Instance.PostAction(done, true, final, chunkX, chunkY, targetPlayer);
        });

        thread.Start();
    }

    public static void GetChunkForNet_Unloaded(GameObject target, string reality, string layer, int chunkX, int chunkY, int chunkSize, UnityAction<object[]> done, UnityAction<string> error = null)
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
                return;
            }

            // RLE:
            // If the chunk is saved using rle, it is sent using rle, and if it is not, then it isn't.

            Threader.Instance.PostAction(done, text, chunkX, chunkY, target);
        });

        thread.Start();
    }

    public static string MakeString(BaseTile[][] tiles, int chunkX, int chunkY, int chunkSize)
    {
        if (tiles == null)
        {
            Debug.LogError("Tiles input is null.");
            return null;
        }

        StringBuilder str = new StringBuilder();

        for (int y = chunkY * chunkSize; y < chunkY * chunkSize + chunkSize; y++)
        {
            for (int x = chunkX * chunkSize; x < chunkX * chunkSize + chunkSize; x++)
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

    public static void LoadChunk(string reality, string layer, int chunkX, int chunkY, int chunkSize, UnityAction<object[]> done, UnityAction<string> error = null, bool rle = true)
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
                return;
            }

            if (rle)
            {
                // Un-RLE this, to return it to its former inefficient glory!
                text = UnRLE(text);
            }

            //Debug.Log("Loaded chunk @ " + chunkX + ", " + chunkY);

            BaseTile[][] tiles = MakeChunk(text, chunkSize, error, false); // Already done in previous step.

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

    public static void MergeAllToFile(string reality, string layer, int chunkSize, int widthInChunks, Dictionary<int, List<NetPendingTile>> operations, UnityAction<object[]> done, bool rle = true)
    {
        // Merges all pending operations to file, saving them forever.
        Thread thread = new Thread(() => 
        {
            foreach (int index in operations.Keys)
            {
                int chunkX = index % widthInChunks;
                int chunkY = index / widthInChunks;

                MergeToFile(reality, layer, chunkX, chunkY, chunkSize, operations[index], rle);
            }

            Threader.Instance.PostAction(done);
        });

        thread.Start();
    }

    public static void MergeToFile(string reality, string layer, int chunkX, int chunkY, int chunkSize, List<NetPendingTile> operations, bool rle = true, char sep = ',', char join = '|')
    {
        // Synchronous. Threaded done in different method.

        // Validate.
        if(!IsChunkSaved(reality, layer, chunkX, chunkY))
        {
            Debug.LogError("Could not merge to file, chunk is not saved! " + reality + ", " + layer + ", (" + chunkX + ", " + chunkY + ")");
            return;
        }

        // Get path.
        string path = GetPathForChunk(reality, layer, chunkX, chunkY);

        // Read from file.
        string text = File.ReadAllText(path);

        // Merge the file with the pending operations.
        string merged = Merge(text, operations, chunkSize, rle);

        // Write back to file.
        File.WriteAllText(path, merged);
    }

    public static string Merge(string data, List<NetPendingTile> operations, int chunkSize, bool rle = true, char separator = ',', char join = '|')
    {
        // Data is a string that is an array that is either plain text or compressed using rle.

        if (rle)
        {
            data = UnRLE(data, separator, join);
        }

        // Split into parts.
        string[] split = data.Split(separator);

        foreach(NetPendingTile op in operations)
        {
            int index = op.X + chunkSize * op.Y;

            split[index] = op.PrefabIsNull() ? NULL_ERROR.ToString() : op.Prefab.Trim();
        }

        // Make back into stirng.
        StringBuilder str = new StringBuilder();
        for(int i = 0; i < split.Length; i++)
        {
            str.Append(split[i]);
            if(i != split.Length - 1)
                str.Append(separator);
        }

        string final = str.ToString();

        // RLE again if necessary.
        if (rle)
        {
            float eff;
            final = RLE(final, out eff, separator, join);
        }

        return final;
    }

    public static string RLE(string original, out float efficiency, char separator = ',', char join = '|', bool trim = true)
    {
        StringBuilder builder = new StringBuilder();
        // Uses Run Length Encoding to compress long repetitive strings.
        // Efficiency is expressed as follows: A value of 1 means a 100% reduction in string size, and a value of zero means a 0% reduction.
        string[] split = original.Split(separator);

        if(split == null || split.Length == 0)
        {
            Debug.LogError("The original string to RLE (below) did not contain the following separator char '" + separator + "'. It must contain at least one.\n" + original);
            efficiency = 0;
            return null;
        }

        builder.Length = 0;

        string last = null;
        int index = 0;
        int run = 1;
        
        foreach(string s in split)
        {
            if(index == split.Length - 1)
            {
                if (s == last)
                {
                    // Just add one to the run count.
                    run++;

                    if (last != null)
                    {
                        if (run > 1)
                        {
                            builder.Append(run);
                            builder.Append(join);
                        }
                        builder.Append(Format(last, trim));
                    }
                    break;
                }
                else
                {
                    // Last one is different from the previous ones, save the previous ones and then save the last one.

                    if (last != null)
                    {
                        if (run > 1)
                        {
                            builder.Append(run);
                            builder.Append(join);
                        }
                        builder.Append(Format(last, trim));
                        builder.Append(separator);
                    }

                    // Now save the last item.

                    // We know that it is only one because it is the last item and it is different from the previous item, may that be null or not.
                    builder.Append(Format(s, trim));

                    break;
                }            
            }

            if(s == last)
            {
                run++;
            }
            else
            {
                if(last == null)
                {
                    run = 1;
                }
                else
                {
                    if(run > 1)
                    {
                        builder.Append(run);
                        builder.Append(join);
                    }
                    builder.Append(Format(last, trim));
                    builder.Append(separator);
                    run = 1;
                }
            }
            last = s;
            index++;
        }

        string str = builder.ToString();
        builder.Length = 0;

        int oldSize = original.Length;
        int newSize = str.Length;
        float fraction = newSize / (float)oldSize;

        efficiency = 1f - fraction;

        //string debug = UnRLE(str, separator, join, trim);

        //if(debug != original)
        //{
        //    Debug.LogError("RLE check failed!\nORIGINAL:\n" + original + "\nUN_RLE:\n" + debug + "\nRLE:\n" + str);
        //}

        return str;
    }

    public static string UnRLE(string rle, char separator = ',', char join = '|', bool trim = true)
    {
        StringBuilder builder = new StringBuilder();

        string[] split = rle.Split(separator);

        if (split == null || split.Length == 0)
        {
            Debug.LogError("The original string to RLE (below) did not contain the following separator char '" + separator + "'. It must contain at least one.\n" + rle);
            return null;
        }

        builder.Length = 0;
        int index = 0;

        foreach(string s in split)
        {
            bool last = index == split.Length - 1;
            if (s.Contains(join))
            {
                string[] subSplit = s.Split(join);

                if(subSplit.Length != 2)
                {
                    Debug.LogError("RLE parsing error at segment '" + s + "' @ " + index + "/" + split.Length + ", incorrect format: Wrong number of joined parts. Expected 2 got " + subSplit.Length + ".");
                    return null;
                }

                string countString = subSplit[0];

                int count;
                bool worked = int.TryParse(countString, out count);

                if (!worked)
                {
                    Debug.LogError("RLE parsing error at segment '" + s + "', invalid input: '" + countString + "' is not a number, expect count value.");
                    return null;
                }

                string value = Format(subSplit[1], trim);

                for (int i = 0; i < count; i++)
                {
                    builder.Append(value);

                    bool veryLast = last && i == count - 1;
                    if(!veryLast)
                        builder.Append(separator);
                }
            }
            else
            {
                builder.Append(Format(s, trim));
                if(!last)
                    builder.Append(separator);
            }

            index++;
        }

        string str = builder.ToString();
        builder.Length = 0;

        return str;
    }

    private static string Format(string s, bool trim)
    {
        if (s != null && trim)
            return s.Trim();

        return s;
    }

    public static BaseTile[][] MakeChunk(string str, int chunkSize, UnityAction<string> error = null, bool rle = true)
    {
        BaseTile[][] chunk = new BaseTile[chunkSize][];

        if(rle && str != null)
        {
            str = UnRLE(str);
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
            chunk[x] = new BaseTile[chunkSize];
            for (int y = 0; y < chunkSize; y++)
            {

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