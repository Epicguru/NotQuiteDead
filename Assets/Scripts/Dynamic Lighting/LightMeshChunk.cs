using UnityEngine;

public class LightMeshChunk : MonoBehaviour
{
    public LightMesh LightMesh;
    public int ChunkX;
    public int ChunkY;
    public Color32 Colour;

    public void ApplyAmbientLight()
    {
        int chunkSize = LightMesh.Gen.Width;
        TileLayer layer = World.Instance.TileMap.GetLayer("Foreground");

        // Pass one: Fully obscure all solid tiles.
        for (int x = -1; x <= chunkSize; x++)
        {
            for (int y = -1; y <= chunkSize; y++)
            {
                int X = (chunkSize * ChunkX) + x;
                int Y = (chunkSize * ChunkY) + y;

                BaseTile tile = layer.GetTile(X, Y);

                // If it is solid...
                if (tile != null)
                {
                    // Set whole tile to darkness.
                    LightMesh.Interaction.SetColour(x, y, LightMeshInteraction.BLACK);
                    LightMesh.Interaction.SetColour(x + 1, y, LightMeshInteraction.BLACK);
                    LightMesh.Interaction.SetColour(x, y + 1, LightMeshInteraction.BLACK);
                    LightMesh.Interaction.SetColour(x + 1, y + 1, LightMeshInteraction.BLACK);
                }
            }
        }

        // Pass two: light up all non-solid tiles, which will overwrite the darkness on the edges of all tiles that are exposed to air, since they
        // share vertices.
        for (int x = -1; x <= chunkSize; x++)
        {
            for (int y = -1; y <= chunkSize; y++)
            {
                int X = (chunkSize * ChunkX) + x;
                int Y = (chunkSize * ChunkY) + y;

                BaseTile tile = layer.GetTile(X, Y);

                // If it is NOT solid...
                if(tile == null)
                {
                    // Set to light!
                    LightMesh.Interaction.SetColour(x, y, Colour);
                    LightMesh.Interaction.SetColour(x + 1, y, Colour);
                    LightMesh.Interaction.SetColour(x, y + 1, Colour);
                    LightMesh.Interaction.SetColour(x + 1, y + 1, Colour);
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