
using Priority_Queue;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Pathfinding
{
    public const int MAX = 1000;
    private static FastPriorityQueue<Node> open = new FastPriorityQueue<Node>(MAX);
    private static Dictionary<Node, Node> cameFrom = new Dictionary<Node, Node>();
    private static Dictionary<Node, float> costSoFar = new Dictionary<Node, float>();
    private static List<Node> near = new List<Node>();

    public static List<Node> Find(int startX, int startY, int endX, int endY, TileLayer layer, bool clean = false)
    {
        open.Clear();
        cameFrom.Clear();
        costSoFar.Clear();

        Node start = new Node() { X = startX, Y = startY };
        Node end = new Node() { X = endX, Y = endY };

        open.Enqueue(start, 0);
        cameFrom[start] = start;
        costSoFar[start] = 0;

        int count = 1;
        while((count = open.Count) > 0)
        {
            if(count > MAX - 9)
            {
                Debug.Log("Too many open nodes!");
                if (clean)
                {
                    Clean();
                }
                return null;
            }

            Node current = open.Dequeue();

            if (current.Equals(end))
            {
                // Done!
                List<Node> path = TracePath(end);
                if (clean)
                {
                    Clean();
                }
                return path;
            }

            List<Node> neighbours = GetNear(current, layer);
            foreach (Node n in neighbours)
            {
                float newCost = costSoFar[current] + GetCost(current, n); // This is the cost of the tile! Could be the weight depending on terrain!
                if(!costSoFar.ContainsKey(n) || newCost < costSoFar[n]) // (effectiveCost ? GetCost(current, n) : 0)
                {
                    costSoFar[n] = newCost;
                    float priority = newCost + Heuristic(n, end);
                    open.Enqueue(n, priority);
                    cameFrom[n] = current;
                }
            }
        }

        if (clean)
        {
            Clean();
        }
        return null;
    }

    private static float GetCost(Node a, Node b)
    {
        // Only intended for neighbours.

        // Is directly horzontal
        if(Mathf.Abs(a.X - b.X) == 1 && a.Y == b.Y)
        {
            return 1;
        }

        // Directly vertical.
        if (Mathf.Abs(a.Y - b.Y) == 1 && a.X == b.X)
        {
            return 1;
        }

        // Assume that it is on one of the corners.
        return 1.41421356237f;
    }

    private static void Clean()
    {
        costSoFar.Clear();
        cameFrom.Clear();
        near.Clear();
    }

    private static List<Node> TracePath(Node end)
    {
        List<Node> path = new List<Node>();
        Node child = end;

        bool run = true;
        while (run)
        {
            Node previous = cameFrom[child];
            path.Add(child);
            if(previous != null && child != previous)
            {
                child = previous;
            }
            else
            {
                run = false;
            }
        }

        path.Reverse();

        return path;
    }

    private static List<Node> GetNear(Node node, TileLayer layer)
    {
        // Want to add nodes connected to the center node, if they are walkable.
        near.Clear();
        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                if (x == 1 && y == 1)
                    continue;

                int X = node.X - 1 + x;
                int Y = node.Y - 1 + y;

                // Make a fast version of getTile.
                BaseTile tile = layer.GetTile(X, Y);

                // If (is not solid) then add to near tiles.
                if(tile == null)
                {
                    near.Add(new Node() { X = X, Y = Y });
                }
            }
        }

        return near;
    }

    private static float Heuristic(Node a, Node b)
    {
        // Gives a rough distance.
        return Mathf.Abs(a.X - b.X) + Mathf.Abs(a.Y - b.Y);
    }
}

public class Node : FastPriorityQueueNode
{
    public int X;
    public int Y;

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