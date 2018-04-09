using UnityEngine;

public class LightMeshChunk : MonoBehaviour
{
    public LightMesh LightMesh;
    public int ChunkX;
    public int ChunkY;
    public Color32 Colour;

    public void LightEmUp()
    {
        int chunkSize = LightMesh.Gen.Width;
        TileLayer layer = World.Instance.TileMap.GetLayer("Foreground");
        for (int x = 0; x < chunkSize; x++)
        {
            for (int y = 0; y < chunkSize; y++)
            {
                int X = (chunkSize * ChunkX) + x;
                int Y = (chunkSize * ChunkY) + y;

                BaseTile tile = layer.GetTile(X, Y);

                bool light = true;
                if(tile != null)
                {
                    light = false;
                }

                if (light)
                {
                    LightMesh.Interaction.SetColour(x, y, Colour);
                    LightMesh.Interaction.SetColour(x + 1, y, Colour);
                    LightMesh.Interaction.SetColour(x, y + 1, Colour);
                    LightMesh.Interaction.SetColour(x + 1, y + 1, Colour);
                }
            }
        }
    }
}