
using UnityEngine;

public class MeshTexture : MonoBehaviour
{
    public int TileResolution = 32;
    public MeshRenderer Renderer;
    public Chunk Chunk;

    public void BuildTexture()
    {
        Texture2D texture = new Texture2D(TileResolution, TileResolution);

        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.Apply();

        Renderer.sharedMaterials[0].mainTexture = texture;
    }

    public void SetTile(Sprite tile, int x, int y)
    {
        if(!Chunk.InBounds(x, y))
        {
            Debug.LogError("Texture blit out of bounds! (" + x + ", " + y + ")");
            return;
        }

        Texture2D t = Renderer.sharedMaterials[0].mainTexture as Texture2D;

        Rect r = tile.textureRect;
        Color[] colours = tile.texture.GetPixels((int)r.x, (int)r.y, (int)r.width, (int)r.height);

        t.SetPixels(x * TileResolution, y * TileResolution, TileResolution, TileResolution, colours);
    }

    public void SetTiles(Sprite tile, int x, int y, int width, int height)
    {
        if(width <= 0 || height <= 0)
        {
            Debug.LogError("Width and height must both be greater that zero! (" + width + ", " + height + ")");
            return;
        }

        for (int X = x; X < x + width; X++)
        {
            for (int Y = y; Y < y + height; Y++)
            {
                if(Chunk.InBounds(X, Y))
                    SetTile(tile, X, Y);
            }
        }
    }
}