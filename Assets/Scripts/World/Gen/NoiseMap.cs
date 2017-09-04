using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class NoiseMap
{
    // Generates a new map of 2D perlin noise.

    private float[] noise;
    private int width, height;

    public NoiseMap(int width, int height)
    {
        if (width <= 0)
            throw new System.Exception("Width cannot be <= zero! (" + width + ").");
        if (height <= 0)
            throw new System.Exception("Height cannot be <= zero! (" + height + ").");

        this.width = width;
        this.height = height;
        Gen();
    }

    private float Noise(int x, int y, float scale, float mag, int seed)
    {
        return Mathf.Clamp((Mathf.PerlinNoise(seed + x / (width * scale), seed + y / (height * scale)) * mag), 0f, 1f);
    }

    private void Gen()
    {
        int seed = Random.Range(0, 1000);
        noise = new float[width * height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float f = Noise(x, y, 0.2f, 1f, seed);
                SetValue(x, y, f);
            }
        }
    }

    private int GetIndex(int x, int y)
    {
        return width * y + x;
    }

    public void SetValue(int x, int y, float value)
    {
        noise[GetIndex(x, y)] = Mathf.Clamp(value, 0f, 1f);
    }

    public float Sample(int x, int y)
    {
        return noise[GetIndex(x, y)];
    }

    public void Multiply(NoiseMap n)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                SetValue(x, y, Sample(x, y) * n.Sample(x, y));
            }
        }
    }

    public void Multiply(Texture2D t)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float z = 0;

                Color c = t.GetPixelBilinear((float)x / width, (float)y / height);
                z = c.r;

                SetValue(x, y, Sample(x, y) * z);
            }
        }
    }
}
