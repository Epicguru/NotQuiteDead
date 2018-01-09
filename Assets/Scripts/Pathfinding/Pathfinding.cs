
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Pathfinding
{
    private static List<Node> open = new List<Node>();
    private static List<Node> closed = new List<Node>();
    private static List<Node> near = new List<Node>();

    public static List<Node> Find(int startX, int startY, int endX, int endY, TileLayer layer)
    {
        open.Clear();
        closed.Clear();

        if (startX < 0 || startY < 0 || endX < 0 || endY < 0)
            return null;

        float g = 0;
        Node start = new Node() { X = startX, Y = startY };
        Node end = new Node() { X = endX, Y = endY };
        Node previous = null;
        open.Add(start);

        int itter = 0;
        while(open.Count > 0)
        {
            Node current = null;
            float lowest = float.MaxValue;
            for (int i = 0; i < open.Count; i++)
            {
                Node n = open[i];
                if(n.F < lowest)
                {
                    lowest = n.F;
                    current = n;
                }
            }

            closed.Add(current);
            open.Remove(current);

            if(current.Equals(end))
            {
                // Done!
                //Debug.Log("Done!");
                Cleanup();
                return TracePath(current);
            }

            // Get neighbours.
            List<Node> near = GetNear(current, layer);
            if(previous != null)
                g += DistanceBetween(current, previous);

            foreach(Node n in near)
            {
                if (closed.Contains(n))
                {
                    continue;
                }

                // Not touched before.
                if (!open.Contains(n))
                {
                    // Calculate score.
                    n.G = g + DistanceBetween(current, n);
                    n.H = ComputeH(n, end);
                    n.F = n.G + n.H;
                    n.Parent = current;
                    open.Insert(0, n);
                }
                else
                {
                    // Possible new path.
                    if(g + n.H < n.F)
                    {
                        n.G = g + DistanceBetween(current, n);
                        n.F = n.G + n.H;
                        n.Parent = current;
                    }
                }
            }
            itter++;
            if(itter >= 10000)
            {
                break;
            }
            previous = current;
        }

        // No path!
        Cleanup();
        return null;
    }

    private static void Cleanup()
    {
        open.Clear();
        closed.Clear();
        near.Clear();
    }

    private static float DistanceBetween(Node a, Node b)
    {
        // Only made to handle adjacent tiles.

        // Directly left or right.
        if(a.X == b.X - 1 || a.X == b.X + 1)
        {
            return 1f;
        }

        // Directly above or below.
        if(a.Y == b.Y - 1 || a.Y == b.Y + 1)
        {
            return 1f;
        }

        // Assume diagonal.
        // Sqrt(horizontal distance squared + diagonal distance squared)
        return 1.41421356f;
    }

    private static List<Node> GetNear(Node spot, TileLayer layer)
    {
        near.Clear();

        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                if (x == 1 && y == 1)
                    continue;

                int X = spot.X - 1 + x;
                int Y = spot.Y - 1 + y;

                if (X < 0 || Y < 0)
                    continue;

                BaseTile tile = layer.GetTile(X, Y);
                if(tile == null) // TODO change this with IF NOT SOLID!
                    near.Add(new Node() { X = X, Y = Y });
            }
        }

        return near;
    }

    private static List<Node> TracePath(Node current)
    {
        List<Node> path = new List<Node>();

        Node c = current;
        while(c != null)
        {
            path.Add(c);
            c = c.Parent;
        }

        return path;
    }

    private static int ComputeH(Node current, Node target)
    {
        return Mathf.Abs(target.X - current.X) - Mathf.Abs(target.Y - current.Y);
    }
}

public class Node
{
    public int X;
    public int Y;

    // G + H
    public float F;

    public float G;
    public float H;
    public Node Parent;

    public override bool Equals(object obj)
    {
        Node other = (Node)obj;
        return other.X == X && other.Y == Y;
    }

    public override int GetHashCode()
    {
        return X + Y * 7;
    }
}