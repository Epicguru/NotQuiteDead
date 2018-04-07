
using System;
using System.Linq;
using UnityEngine;

public class MeshTexture : MonoBehaviour
{
    // IMPORTANT TODO: Use Color32.
    // IMPORTANT TODO: Cache flipped colour data, probably in the BaseTile itself.

    [Tooltip("The resolution of pixels of each tile on this chunk.")]
    public int TileResolution = 32;

    public bool EnforceResolution;

    public static Color[] Transparent;
    private static Color TransparentPixel = new Color(0, 0, 0, 0);

    public MeshRenderer Renderer;
    public Chunk Chunk;

    public bool Mipmaps;
    public FilterMode FilterMode;

    public bool Dirty { get; set; }

    private static Color[] TransparentTile;
    private Texture2D Texture;

    public void BuildTexture()
    {
        Texture2D texture = new Texture2D(TileResolution * Chunk.Width, TileResolution * Chunk.Height, TextureFormat.ARGB32, Mipmaps);

        if(Transparent == null || Transparent.Length != (Chunk.Width * Chunk.Height * TileResolution * TileResolution))
        {
            Transparent = new Color[Chunk.Width * Chunk.Height * TileResolution * TileResolution];
            for (int i = 0; i < Transparent.Length; i++)
            {
                Transparent[i] = TransparentPixel;
            }
        }

        texture.SetPixels(Transparent);

        texture.filterMode = FilterMode;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.Apply();

        this.Texture = texture;

        Renderer.material.mainTexture = texture;
    }

    public Color[] GetTransparentTile()
    {
        if(TransparentTile == null)
        {
            TransparentTile = new Color[TileResolution * TileResolution];
            Color transparent = new Color(0, 0, 0, 0);
            for (int i = 0; i < TransparentTile.Length; i++)
            {
                TransparentTile[i] = transparent;
            }
        }

        return TransparentTile;
    }

    public Color[] GetColours(Sprite sprite, bool packSafe = true, bool flipY = true)
    {
        if(sprite == null)
        {
            Debug.LogError("Sprite input is null, cannot extract colours!");
            return null;
        }

        Color[] pixels;

        if (packSafe)
        {
            Rect r = sprite.textureRect;
            try
            {           
                pixels = sprite.texture.GetPixels((int)r.x, (int)r.y, (int)r.width, (int)r.height);
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to get colours from sprite. (Packing safe) Exception: " + e.Message + ", Sprite: " + sprite.name);
                return null;
            }
        }
        else
        {
            try
            {
                pixels = sprite.texture.GetPixels();

            } catch (Exception e)
            {
                Debug.LogError("Failed to get colours from sprite. (NOT packing safe) Exception: " + e.Message + ", Sprite: " + sprite.name);
                return null;
            }
        }

        if (flipY)
        {
            Rect r = sprite.textureRect;
            pixels = FlipPixels(pixels, (int)r.width, (int)r.height, false, true);
        }

        return pixels;
    }

    /// <summary>
    /// Sets the pixels of this sprite to the specified position. Does not cache the operation result and therefore is very inefficient.
    /// </summary>
    /// <param name="tile">The sprite to use.</param>
    /// <param name="x">The local X position.</param>
    /// <param name="y">The local Y position.</param>
    public void SetTile(Sprite tile, int x, int y)
    {
        Color[] colours;

        if (tile == null)
        {
            colours = GetTransparentTile();
        }
        else
        {
            colours = GetColours(tile);
        }

        if (colours == null)
            return;

        SetTile(colours , x, y);
    }

    /// <summary>
    /// Sets the pixels corresponding to the tile in its current surroundings. None of the parameters can be null apart from 'tile'.
    /// Caches the operation so future calls with the same BaseTile will be very quick.
    /// </summary>
    public void SetTile(BaseTile tile, TileLayer layer, int globalX, int globalY, int x, int y)
    {
        if(tile == null)
        {
            SetTile(GetTransparentTile(), x, y);
        }
        else
        {
            int index = tile.GetAutoIndex(layer, globalX, globalY);

            if (tile.RenderData.IsCached(index))
            {
                // Already cached, just paint.
                SetTile(tile.RenderData.GetCachedPixels(index), x, y);
            }
            else
            {
                // Need to grab and flip the pixels, the store them again.
                tile.RenderData.SetCachedPixels(index, GetColours(tile.RenderData.GetSprite(index), true, true));

                // Now paint the tile.
                SetTile(tile.RenderData.GetCachedPixels(index), x, y);
            }
        }
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
            Debug.LogError("Incorrect number of pixels when blitting tile. There were " + colours.Length + " but there should have been " + TileResolution * TileResolution + ". Tile resolution should be " + TileResolution + "x" + TileResolution + ".");
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
        Color[] colours;

        if (tile == null)
        {
            colours = GetTransparentTile();
        }
        else
        {
            colours = GetColours(tile);
        }

        if (colours == null)
            return;

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

    public static Color[] FlipPixels(Color[] pixels, int width, int height, bool flipX, bool flipY)
    {
        var flippedPixels =
            Enumerable.Range(0, width * height)
            .Select(index => {
                int x = index % width;
                int y = index / width;
                if (flipX)
                    x = width - 1 - x;
                if (flipY)
                    y = height - 1 - y;
                return pixels[y * width + x];
            }
        );

        return flippedPixels.ToArray();
    }

    public void Apply()
    {
        if (!Chunk.Loaded)
        {
            // Do not apply the texture when loading the chunk, does not make sense.
            return;
        }

        if (Texture != Renderer.material.mainTexture)
        {
            Texture = Renderer.material.mainTexture as Texture2D;
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