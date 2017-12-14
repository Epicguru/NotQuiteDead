using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static class ChunkIO
{
    public static string MakeString(BaseTile[][] tiles, int chunkX, int chunkY, int chunkSize)
    {
        if (tiles == null)
        {
            Debug.LogError("Tiles input is null.");
            return null;
        }

        for (int x = chunkX * chunkSize; x < chunkX * chunkSize + chunkSize; x++)
        {
            for (int y = chunkY * chunkSize; y < chunkY * chunkSize + chunkSize; y++)
            {
                BaseTile tile = 
            }
        }
    }
}