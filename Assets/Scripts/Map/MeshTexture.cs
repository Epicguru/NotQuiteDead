
using System;
using UnityEngine;

public class MeshTexture : MonoBehaviour
{
    [Tooltip("The resolution of pixels of each tile on this chunk.")]
    public int TileResolution = 32;

    public bool EnforceResolution;

    public static Color[] Transparent;
    private static Color TransparentPixel = new Color(0, 0, 0, 0);

    public MeshRenderer Renderer;
    public Chunk Chunk;

    public bool Mipmaps, Linear;

    public bool Dirty { get; set; }

    private Texture2D Texture;

    public void BuildTexture()
    {
        Texture2D texture = new Texture2D(TileResolution * Chunk.Width, TileResolution * Chunk.Height, TextureFormat.ARGB32, Mipmaps, Linear);

        if(Transparent == null || Transparent.Length != (Chunk.Width * Chunk.Height * TileResolution * TileResolution))
        {
            Transparent = new Color[Chunk.Width * Chunk.Height * TileResolution * TileResolution];
            for (int i = 0; i < Transparent.Length; i++)
            {
                Transparent[i] = TransparentPixel;
            }
        }

        texture.SetPixels(Transparent);

        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.Apply();

        this.Texture = texture;

        Renderer.sharedMaterials[0].mainTexture = texture;
    }

    public Color[] GetColours(Sprite sprite, bool packSafe = true)
    {
        if(sprite == null)
        {
            Debug.LogError("Sprite input is null, cannot extract colours!");
            return null;
        }

        if (packSafe)
        {
            Rect r = sprite.textureRect;
            try
            {
                return sprite.texture.GetPixels((int)r.x, (int)r.y, (int)r.width, (int)r.height);
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to get colours from sprite. Exception: " + e.Message + ", Sprite: " + sprite.name);
                return null;
            }
        }
        else
        {
            return sprite.texture.GetPixels();
        }
    }

    public void SetTile(Sprite tile, int x, int y)
    {
        if(tile == null)
        {
            Debug.LogError("Null sprite input when blitting tile.");
            return;
        }

        Color[] colours = GetColours(tile);

        if (colours == null)
            return;

        SetTile(colours , x, y);
    }

    public void SetTile(Color[] colours, int x, int y)
    {
        if(!Chunk.InBounds(x, y))
        {
            Debug.LogError("Texture blit out of bounds! (" + x + ", " + y + ")");
            return;
        }

        if(colours == null)
        {
            Debug.LogError("Null colours when blitting tile!");
            return;
        }

        if(EnforceResolution && colours.Length != TileResolution * TileResolution)
        {
            Debug.LogError("Too many pixels when blitting tile. There were " + colours.Length + " but there should have been " + TileResolution * TileResolution + ". Tile resolution should be " + TileResolution + "x" + TileResolution + ".");
            return;
        }

        if (Texture == null)
        {
            Debug.LogError("Texture is null! Cannot write anything until texture has been built!");
            return;
        }

        Texture.SetPixels(x * TileResolution, (Chunk.Height - 1 - y) * TileResolution, TileResolution, TileResolution, colours);
        Dirty = true;
    }

    public void SetTiles(Color[] colours, int x, int y, int width, int height)
    {
        if (width <= 0 || height <= 0)
        {
            Debug.LogError("Width and height must both be greater that zero! (" + width + ", " + height + ")");
            return;
        }

        if (colours == null)
        {
            Debug.LogError("Null colours when blitting tiles (multiple) !");
            return;
        }

        for (int X = x; X < x + width; X++)
        {
            for (int Y = y; Y < y + height; Y++)
            {
                if (Chunk.InBounds(X, Y))
                    SetTile(colours, X, Y);
            }
        }
    }

    public void SetTiles(Sprite tile, int x, int y, int width, int height)
    {
        if(tile == null)
        {
            Debug.LogError("Null sprite when blitting tiles (multiple) !");
            return;
        }

        Color[] colours = GetColours(tile);

        // We know that colours is not null because the sprite is not null!

        SetTiles(colours, x, y, width, height);
    }

    public void SetPixel(Color colour, int x, int y)
    {
        if(Texture == null)
        {
            Debug.LogError("Texture is null! Cannot write anything until texture has been built!");
            return;
        }

        Texture.SetPixel(x, y, colour);
        Dirty = true;
    }

    public void Apply()
    {
        if (Texture != Renderer.sharedMaterials[0].mainTexture)
        {
            Texture = Renderer.sharedMaterials[0].mainTexture as Texture2D;
        }

        if(Texture == null)
        {
            Debug.LogError("Chunk texture is null, cannot apply! Must build texture so that there is something to apply!");
            return;
        }

        if (!Dirty)
        {
            Debug.LogError("Chunk texture is not dirty. Cannot apply texture.");
            return;
        }

        Texture.Apply();

        Dirty = false;
    }
}