using UnityEngine;

public class LightMeshChunk : MonoBehaviour
{
    public LightMesh LightMesh;
    public int ChunkWidth = 16;
    public int ChunkHeight = 16;
    public int ChunkX;
    public int ChunkY;
    public int VERTS_PER_TILE;
    public Color32 LightColour;
    public MeshRenderer Renderer;

    public void Update()
    {
        Renderer.material.SetColor("_AmbientColour", AmbientLight.Instance.GetCurrentColour());
    }

    public void ApplyAmbientLight()
    {
        int width = ChunkWidth;
        int height = ChunkHeight;
        TileLayer layer = World.Instance.TileMap.GetLayer("Foreground");

        Color32 dark = LightMesh.Interaction.Shadow;

        // First, fill the chunk with the ambient light colour...
        LightMesh.Interaction.Fill(LightColour);

        // Now do a world interaction pass: darken tiles that are solid.
        for (int x = -1; x <= width; x++)
        {
            for (int y = -1; y <= height; y++)
            {
                int tileX = (width * ChunkX) + x;
                int tileY = (height * ChunkY) + y;

                BaseTile tile = layer.Unsafe_GetTile(tileX, tileY);

                // If is 'solid'
                if (tile != null)
                {
                    // Calc vert x and y
                    int vertX = x * VERTS_PER_TILE;
                    int vertY = y * VERTS_PER_TILE;
                    // Set tile to darkness...
                    LightMesh.Interaction.SetBox(vertX, vertY, vertX + VERTS_PER_TILE, vertY + VERTS_PER_TILE, dark);                  
                }
            }
        }

        // Apply ambient light to all non-solid tiles.
        for (int x = -1; x <= width; x++)
        {
            for (int y = -1; y <= height; y++)
            {
                int tileX = (width * ChunkX) + x;
                int tileY = (height * ChunkY) + y;

                BaseTile tile = layer.Unsafe_GetTile(tileX, tileY);

                // If is 'air'
                if (tile == null)
                {
                    // Calc vert x and y
                    int vertX = x * VERTS_PER_TILE;
                    int vertY = y * VERTS_PER_TILE;
                    // Set tile to darkness...
                    LightMesh.Interaction.SetBox(vertX, vertY, vertX + VERTS_PER_TILE, vertY + VERTS_PER_TILE, LightColour);
                }
            }
        }

        // Remove darkness from tiles that have their corners lit. Looks much better.
        for (int x = -1; x <= width; x++)
        {
            for (int y = -1; y <= height; y++)
            {
                int tileX = (width * ChunkX) + x;
                int tileY = (height * ChunkY) + y;

                BaseTile tile = layer.Unsafe_GetTile(tileX, tileY);

                // If is 'solid'
                if (tile != null)
                {
                    // Calc bottom left vert x and y
                    int vertX = x * VERTS_PER_TILE;
                    int vertY = y * VERTS_PER_TILE;

                    int lit = 0;

                    // Bottom left
                    if(LightMesh.Interaction.GetColour(vertX, vertY).IsEqual(LightColour))
                    {
                        // It is lit!
                        lit++;
                    }

                    // Bottom right
                    if (LightMesh.Interaction.GetColour(vertX + VERTS_PER_TILE, vertY).IsEqual(LightColour))
                    {
                        // It is lit!
                        lit++;
                    }

                    // Top left
                    if (LightMesh.Interaction.GetColour(vertX, vertY + VERTS_PER_TILE).IsEqual(LightColour))
                    {
                        // It is lit!
                        lit++;
                    }

                    // Top right
                    if (LightMesh.Interaction.GetColour(vertX + VERTS_PER_TILE, vertY + VERTS_PER_TILE).IsEqual(LightColour))
                    {
                        // It is lit!
                        lit++;
                    }

                    if(lit >= 4)
                    {
                        // Set the tile to light...
                        LightMesh.Interaction.SetBox(vertX, vertY, vertX + VERTS_PER_TILE, vertY + VERTS_PER_TILE, LightColour);
                    }
                }
            }
        }

        // Apply to the mesh.
        LightMesh.Interaction.Apply();
    }
}

public static class Color32Extensions
{
    public static bool IsEqual(this Color32 colour, Color32 other, bool alpha = true)
    {
        return colour.r == other.r && colour.g == other.g && colour.b == other.b && (!alpha || colour.a == other.a);
    }
}