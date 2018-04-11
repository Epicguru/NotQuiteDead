using UnityEngine;

public class LightMeshChunk : MonoBehaviour
{
    public LightMesh LightMesh;
    public int ChunkWidth = 16;
    public int ChunkHeight = 16;
    public int ChunkX;
    public int ChunkY;
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

                // If it is solid...
                if (tile != null)
                {
                    // Calc vert x and y
                    int vertX = x * 2 + 1;
                    int vertY = y * 2 + 1;

                    // Set tile to darkness...
                    LightMesh.Interaction.SetColour(vertX, vertY, dark);                    
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