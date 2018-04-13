using UnityEngine;

public class LightMeshChunk : MonoBehaviour
{
    public LightMesh LightMesh;
    public int ChunkWidth = 16;
    public int ChunkHeight = 16;
    public int ChunkX;
    public int ChunkY;
    public int VERTS_PER_TILE;
    public Color32 Colour;

    public void ApplyAmbientLight()
    {
        int width = ChunkWidth;
        int height = ChunkHeight;
        TileLayer layer = World.Instance.TileMap.GetLayer("Foreground");

        Color32 dark = LightMeshInteraction.BLACK;

        // First, fill the chunk with the ambient light colour...
        LightMesh.Interaction.Fill(Colour);

        // Now do a world interaction pass: darken tiles that are solid.
        for (int x = -1; x <= width; x++)
        {
            for (int y = -1; y <= height; y++)
            {
                int tileX = (width * ChunkX) + x;
                int tileY = (height * ChunkY) + y;

                BaseTile tile = layer.GetTile(tileX, tileY);
                BaseTile r = layer.GetTile(tileX + 1, tileY);
                BaseTile l = layer.GetTile(tileX - 1, tileY);
                BaseTile u = layer.GetTile(tileX, tileY + 1);
                BaseTile d = layer.GetTile(tileX, tileY - 1);
                int solid = 0;
                if (r != null) solid++;
                if (l != null) solid++;
                if (u != null) solid++;
                if (d != null) solid++;

                if (solid < 3)
                    continue;

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

                BaseTile tile = layer.GetTile(tileX, tileY);

                // If is 'air'
                if (tile == null)
                {
                    // Calc vert x and y
                    int vertX = x * VERTS_PER_TILE;
                    int vertY = y * VERTS_PER_TILE;
                    // Set tile to darkness...
                    LightMesh.Interaction.SetBox(vertX, vertY, vertX + VERTS_PER_TILE, vertY + VERTS_PER_TILE, Colour);
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