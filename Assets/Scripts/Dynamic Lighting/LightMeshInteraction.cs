using UnityEngine;

public class LightMeshInteraction : MonoBehaviour
{
    public MeshFilter Filter;
    public LightMeshGen Gen;
    public bool SuppressBoundsWarnings;

    public static Color32 BLACK = new Color32(0, 0, 0, 0);

    private Color32[] CacheColours;

    private Color32[] GetColours()
    {
        if(CacheColours == null)
        {
            if (Filter.mesh.colors32 == null || Filter.mesh.colors32.Length != Filter.mesh.vertexCount)
            {
                CacheColours = new Color32[Filter.mesh.vertexCount];
                Filter.mesh.colors32 = CacheColours;
                return CacheColours;
            }
            else
            {
                CacheColours = Filter.mesh.colors32;
                return CacheColours;
            }
        }
        else
        {
            if(CacheColours.Length != Filter.mesh.vertexCount)
            {
                CacheColours = new Color32[Filter.mesh.vertexCount];
                Filter.mesh.colors32 = CacheColours;
                return CacheColours;

            }
            return CacheColours;
        }
    }

    public void SetColour(int x, int y, Color32 colour)
    {
        if (x < 0 || y < 0)
        {
            LogOutOfBounds(x, y);
            return;
        }

        if (x > Gen.Width || y > Gen.Height)
        {
            LogOutOfBounds(x, y);
            return;
        }

        var colours = GetColours();
        SetColourInternal(colours, x, y, colour);
    }

    private void SetColourInternal(Color32[] colours, int x, int y, Color32 c)
    {
        int index = x + (y * (Gen.Width + 1));
        colours[index] = c;
    }

    public void Fill(Color32 colour)
    {
        var colours = GetColours();
        for (int x = 0; x <= Gen.Width; x++)
        {
            for (int y = 0; y <= Gen.Height; y++)
            {
                SetColourInternal(colours, x, y, colour);
            }
        }
    }

    public void Apply()
    {
        Filter.mesh.colors32 = CacheColours;
    }

    public void SetEdgeColours(int x, int y, int x2, int y2, Color32 a, Color32 b)
    {
        // Each tile is 4 vertices
        if (x < 0 || y < 0)
        {
            LogOutOfBounds(x, y);
            return;
        }

        if (x > Gen.Width || y > Gen.Height)
        {
            LogOutOfBounds(x, y);
            return;
        }

        if (x2 < 0 || y2 < 0)
        {
            LogOutOfBounds(x, y);
            return;
        }

        if (x2 > Gen.Width || y2 > Gen.Height)
        {
            return;
        }

        bool hor = false;
        bool vert = false;
        if(x != x2)
        {
            // Is a horizontal edge...
            hor = true;
        }
        if(y != y2)
        {
            // Is a vertical edge
            vert = true;
        }
        
        if(!hor && !vert)
        {
            Debug.LogWarning("Tried to colour an edge but provided a point: ({0}, {1})".Form(x, y));
            return;
        }
        if(hor && vert)
        {
            Debug.LogWarning("Tried to colour an edge put provided a line. Edge must be vertical or horizontal only. (From {0}, {1} to {2}, {3}".Form(x, y, x2, y2));
            return;
        }

        if (hor)
        {
            for (int X = x; X <= x2; X++)
            {
                float p = (float)X / x2;
                SetColour(X, y, Color32.Lerp(a, b, p));
            }
            return;
        }

        if (vert)
        {
            for (int Y = y; Y <= y2; Y++)
            {
                float p = (float)Y / y2;
                SetColour(x, Y, Color32.Lerp(a, b, p));
            }
            return;
        }
    }

    public void SetEdgeColour(int x, int y, int x2, int y2, Color32 a)
    {
        // Each tile is 4 vertices
        if (x < 0 || y < 0)
        {
            LogOutOfBounds(x, y);
            return;
        }

        if (x > Gen.Width || y > Gen.Height)
        {
            LogOutOfBounds(x, y);
            return;
        }

        if (x2 < 0 || y2 < 0)
        {
            LogOutOfBounds(x, y);
            return;
        }

        if (x2 > Gen.Width || y2 > Gen.Height)
        {
            return;
        }

        bool hor = false;
        bool vert = false;
        if (x != x2)
        {
            // Is a horizontal edge...
            hor = true;
        }
        if (y != y2)
        {
            // Is a vertical edge
            vert = true;
        }

        if (!hor && !vert)
        {
            Debug.LogWarning("Tried to colour an edge but provided a point: ({0}, {1})".Form(x, y));
            return;
        }
        if (hor && vert)
        {
            Debug.LogWarning("Tried to colour an edge put provided a line. Edge must be vertical or horizontal only. (From {0}, {1} to {2}, {3}".Form(x, y, x2, y2));
            return;
        }

        if (hor)
        {
            for (int X = x; X <= x2; X++)
            {
                SetColour(X, y, a);
            }
            return;
        }

        if (vert)
        {
            for (int Y = y; Y <= y2; Y++)
            {
                SetColour(x, Y, a);
            }
            return;
        }
    }

    public Color32 GetColour(int x, int y)
    {
        if (x < 0 || y < 0)
        {
            LogOutOfBounds(x, y);
            return BLACK;
        }

        if (x > Gen.Width || y > Gen.Height)
        {
            LogOutOfBounds(x, y);
            return BLACK;
        }

        var colours = GetColours();
        int index = x + (y * (Gen.Width + 1));

        return colours[index];
    }

    private void LogOutOfBounds(int x, int y)
    {
        if(!SuppressBoundsWarnings)
            Debug.LogWarning("X: {0} or Y: {1} are out of mesh vertex bounds! Width: {2}, Height: {3}".Form(x, y, Gen.Width, Gen.Height));
    }
}