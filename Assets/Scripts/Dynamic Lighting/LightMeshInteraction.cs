using UnityEngine;

public class LightMeshInteraction : MonoBehaviour
{
    public MeshFilter Filter;
    public LightMeshGen Gen;

    private static Color32 BLANK_COLOUR = new Color32(0, 0, 0, 0);

    private Color32[] GetColours()
    {
        if (Filter == null)
            return null;
        if (Filter.mesh == null)
            return null;

        if(Filter.mesh.colors32 == null)
        {
            Filter.mesh.colors32 = new Color32[Filter.mesh.vertexCount];
            return Filter.mesh.colors32;
        }
        else
        {
            if(Filter.mesh.colors32.Length != Filter.mesh.vertexCount)
            {
                Filter.mesh.colors32 = new Color32[Filter.mesh.vertexCount];
                return Filter.mesh.colors32;
            }
            else
            {
                return Filter.mesh.colors32;
            }
        }
    }

    public void SetColour(int x, int y, Color32 colour)
    {
        if (x < 0 || y < 0)
        {
            Debug.LogWarning("X: {0} or Y: {1} are out of mesh vertex bounds! Width: {2}, Height: {3}".Form(x, y, Gen.Width, Gen.Height));
            return;
        }

        if (x > Gen.Width || y > Gen.Height)
        {
            Debug.LogWarning("X: {0} or Y: {1} are out of mesh vertex bounds! Width: {2}, Height: {3}".Form(x, y, Gen.Width, Gen.Height));
            return;
        }

        var colours = GetColours();
        int index = x + (y * (Gen.Width + 1));

        colours[index] = colour;

        Filter.mesh.colors32 = colours;
    }

    public Color32 GetColour(int x, int y)
    {
        if (x < 0 || y < 0)
        {
            Debug.LogWarning("X: {0} or Y: {1} are out of mesh vertex bounds! Width: {2}, Height: {3}".Form(x, y, Gen.Width, Gen.Height));
            return BLANK_COLOUR;
        }

        if (x > Gen.Width || y > Gen.Height)
        {
            Debug.LogWarning("X: {0} or Y: {1} are out of mesh vertex bounds! Width: {2}, Height: {3}".Form(x, y, Gen.Width, Gen.Height));
            return BLANK_COLOUR;
        }

        var colours = GetColours();
        int index = x + (y * (Gen.Width + 1));

        return colours[index];
    }
}