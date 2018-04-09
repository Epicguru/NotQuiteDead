﻿using UnityEngine;

public class LightMeshInteraction : MonoBehaviour
{
    public MeshFilter Filter;
    public LightMeshGen Gen;
    public bool SuppressBoundsWarnings;

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
            LogOutOfBounds(x, y);
            return;
        }

        if (x > Gen.Width || y > Gen.Height)
        {
            LogOutOfBounds(x, y);
            return;
        }

        var colours = GetColours();
        int index = x + (y * (Gen.Width + 1));

        colours[index] = colour;

        Filter.mesh.colors32 = colours;
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

    public Color32 GetColour(int x, int y)
    {
        if (x < 0 || y < 0)
        {
            LogOutOfBounds(x, y);
            return BLANK_COLOUR;
        }

        if (x > Gen.Width || y > Gen.Height)
        {
            LogOutOfBounds(x, y);
            return BLANK_COLOUR;
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