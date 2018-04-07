using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

public class WorldGen : MonoBehaviour
{
    // Generates a world's tiles, chunk background, that kind of stuff...

    public float Smallest = 1f;
    public float Largest = 0f;

    private float GetNoise(int x, int y, int width, int height, int seed, float scale)
    {
        float noise = 0f;

        float startX = 0f;
        float startY = 0f;

        float xCoord = (startX + x) / width * scale;
        float yCoord = (startY + y) / height * scale;

        noise = Mathf.PerlinNoise(xCoord, yCoord);

        if (noise > Largest)
        {
            Largest = noise;
        }
        if(noise < Smallest)
        {
            Smallest = noise;
        }
        return noise;
    }

    public void GenBackgrounds(World world, int seed)
    {
        Profiler.BeginSample("Generate Backgrounds");

        int width = world.TileMap.WidthInChunks;
        int height = world.TileMap.HeightInChunks;

        string[] array = new string[width * height];

        float[][] heightmap = new float[width][];

        float scale = 50f;

        for (int x = 0; x < width; x++)
        {
            heightmap[x] = new float[height];
            for (int y = 0; y < height; y++)
            {
                heightmap[x][y] = GetNoise(x, y, width, height, seed, scale);                
            }
        }

        TileLayer foreground = world.TileMap.GetLayer("Foreground");
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float h = heightmap[x][y];
                int index = foreground.GetChunkIndex(x, y);

                //if(h >= 0.5f)
                //{
                //    array[index] = "Dirt";
                //}
                //else
                //{
                //    array[index] = "Grass";
                //}

                if (h >= 0.55f)
                {
                    array[index] = "Grass";
                }
                else if (h >= 0.3f)
                {
                    array[index] = "Dirt";
                }
                else
                {
                    array[index] = "Sand";
                }
            }
        }

        foreground.ChunkBackgrounds = array;

        Profiler.EndSample();
        Debug.Log("Generated backgrounds (w:{0}, h:{1}, s:{2})".Form(width, height, scale));
    }
}