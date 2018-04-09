using UnityEngine;

public class LightMesh : MonoBehaviour
{
    public LightMeshGen Gen;
    public LightMeshInteraction Interaction;

    [Header("Vertex Colour Set")]
    public int Range;
    public Color32 ColourA;
    public Color32 ColourB;
    public int X, Y;

    public void RunColourSet()
    {
        Vector2 origin = new Vector2(X, Y);

        for (int x = -Range; x <= Range; x++)
        {
            for (int y = -Range; y <= Range; y++)
            {
                float dst = Vector2.Distance(origin, new Vector2(X + x, Y + y));
                float p = dst / Range;
                float strength = 1f - p;

                Interaction.SetColour(X + x, Y + y, Color32.Lerp(Interaction.GetColour(X + x, Y + y), ColourA, strength));
            }
        }
    }

    public void RunEdgeSet()
    {
        Interaction.SetEdgeColours(0, 0, 0, Gen.Height, ColourA, ColourB);
    }
}