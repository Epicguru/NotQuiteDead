
using System.Collections.Generic;
using UnityEngine;

public static class Pathfinding
{
    private static List<Node> open = new List<Node>();
    private static List<Node> closed = new List<Node>();
    private static Dictionary<int, Node> cameFrom = new Dictionary<int, Node>();
    private static Dictionary<int, float> gScores = new Dictionary<int, float>();
    private static Dictionary<int, float> fScores = new Dictionary<int, float>();
    private static Node[] neighbours = new Node[4];
    private static Node empty = new Node(false);

    public static void Find(int startX, int startY, int endX, int endY, TileLayer layer)
    {
        open.Clear();
        closed.Clear();
        cameFrom.Clear();
        gScores.Clear();
        fScores.Clear();

        Node start = new Node(startX, startY, layer.Width);
        Node end = new Node(endX, endY, layer.Width);

        open.Add(start);

        gScores[start.Index] = 0f;
        fScores[start.Index] = HeuristicCost(start, end);

        while(open.Count != 0)
        {
            Node current = LowestFscore();
            if(current.SameAs(end))
            {
                // DONE!
                // TODO reconstruct path
                Debug.Log("Found path!");
            }

            open.Remove(current);
            closed.Add(current);

            // Get the neighbours
            GetNeighbours(current, layer);

            for (int i = 0; i < 4; i++)
            {
                Node n = neighbours[i];
                if (n.SameAs(empty))
                    continue;

                if (closed.Contains(n))
                {
                    continue;
                }

                if (!open.Contains(n))
                {
                    open.Add(n);
                }

                float gscore = gScores[current.Index] + DistanceBetween(current, n);
                if (gscore >= gScores[n.Index])
                {
                    continue;
                }

                cameFrom[n.Index] = current;
                gScores[n.Index] = gscore;
                fScores[n.Index] = gscore + HeuristicCost(n, end);
            }
        }
    }

    private static float HeuristicCost(Node a, Node b)
    {
        // TODO.
        return 1f;
    }

    private static float DistanceBetween(Node a, Node b)
    {
        // TODO.
        return 1f;
    }

    private static void GetNeighbours(Node center, TileLayer layer)
    {
        // Left
        BaseTile tile = layer.GetTile(center.X - 1, center.Y);
        neighbours[0] = tile == null ? empty : new Node(center.X - 1, center.Y, layer.Width);

        // Top
        tile = layer.GetTile(center.X, center.Y + 1);
        neighbours[1] = tile == null ? empty : new Node(center.X, center.Y + 1, layer.Width);

        // Right
        tile = layer.GetTile(center.X + 1, center.Y);
        neighbours[2] = tile == null ? empty : new Node(center.X + 1, center.Y, layer.Width);

        // Bottom
        tile = layer.GetTile(center.X, center.Y - 1);
        neighbours[3] = tile == null ? empty : new Node(center.X, center.Y - 1, layer.Width);
    }

    private static Node LowestFscore()
    {
        // Maybe optimise? Score of 0 automatically wins?

        float lowest = float.MaxValue;
        Node lowestNode = open[0];

        foreach (Node n in open)
        {
            if (fScores.ContainsKey(n.Index))
            {
                float score = fScores[n.Index];
                if (score < lowest)
                {
                    lowest = score;
                    lowestNode = n;
                }
            }
        }

        return lowestNode;
    }
}

internal struct Node
{
    public Node(int x, int y, int width)
    {
        X = x;
        Y = y;
        Index = X + Y * width;
    }

    public Node(bool n)
    {
        X = -1;
        Y = -1;
        Index = -1;
    }

    public override bool Equals(object obj)
    {
        return SameAs((Node)obj);
    }

    public override int GetHashCode()
    {
        return Index;
    }

    public bool SameAs(Node other)
    {
        return other.Index == this.Index;
    }

    public int X;
    public int Y;
    public int Index;
}