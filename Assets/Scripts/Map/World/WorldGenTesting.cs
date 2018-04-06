using UnityEngine;

[ExecuteInEditMode]
public class WorldGenTesting : MonoBehaviour
{
    public Texture2D Island;

    [Header("Generation")]
    public int Width;
    public int Height;
    public float Scale;
    public float OffX;
    public float OffY;

    [Range(0f, 1f)]
    public float LowerCutoff;

    [Range(0f, 1f)]
    public float HighCutoff;

    public MeshRenderer Renderer;

    public void Update()
    {
        var mask = LoadFromTexture(Width, Height, Island);
        var noise = MakeHeightMap(Width, Height, OffX, OffY, Scale);
        var x = Multiply(Width, Height, noise, mask);
        x = CutoffLow(Width, Height, LowerCutoff, x);
        x = CutoffHigh(Width, Height, HighCutoff, x);

        SetTexture(Width, Height, x);
        //SetTexture(mask.Length, mask[0].Length, mask);
    }

    public void SetTexture(int width, int height, float[][] heights)
    {
        var tex = new Texture2D(width, height);
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float h = heights[x][y];
                tex.SetPixel(x, y, new Color(h, h, h, 1f));
            }
        }
        tex.filterMode = FilterMode.Point;
        tex.Apply();
        Renderer.material.mainTexture = tex;
    }

    public float[][] CutoffLow(int width, int height, float minValue, float[][] noise)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float h = noise[x][y];

                if (h < minValue)
                {
                    h = 0f;
                }

                noise[x][y] = h;
            }
        }

        return noise;
    }

    public float[][] CutoffHigh(int width, int height, float maxValue, float[][] noise)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float h = noise[x][y];

                if (h > maxValue)
                {
                    h = 1f;
                }

                noise[x][y] = h;
            }
        }

        return noise;
    }

    public float[][] LoadFromTexture(int width, int height, Texture2D texture)
    {
        float[][] h = new float[width][];

        for (int x = 0; x < width; x++)
        {
            h[x] = new float[height];
            for (int y = 0; y < height; y++)
            {
                h[x][y] = texture.GetPixelBilinear((float)x / width,(float)y / height).r;
            }
        }

        return h;
    }

    public float[][] Multiply(int width, int height, float[][] destination, float[][] source)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                destination[x][y] *= source[x][y];
            }
        }

        return destination;
    }

    public float[][] MakeHeightMap(int width, int height, float offX, float offY, float scale)
    {
        float[][] noises = new float[width][];

        for (int x = 0; x < width; x++)
        {
            noises[x] = new float[height];
            for (int y = 0; y < height; y++)
            {
                float noise = 1f;

                float xCoord = (offX + x) / width * scale;
                float yCoord = (offY + y) / height * scale;

                noise = Mathf.Clamp(Mathf.PerlinNoise(xCoord, yCoord), 0f, 1f);

                noises[x][y] = noise;
            }
        }

        return noises;
    }
}