using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathTest : MonoBehaviour {

    public Vector2Int Start_;
    public Vector2Int End;
    public TileLayer Layer;

    public List<Node> path = new List<Node>();

    public void Update()
    {
        path = Pathfinding.Find(Start_.x, Start_.y, End.x, End.y, Layer);
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;

        if(path == null)
        {
            return;
        }

        for (int i = 0; i < path.Count; i++)
        {
            Node last = null;
            if(i != 0)
            {
                last = path[i - 1];
            }
            Node current = path[i];

            if (last != null)
                Gizmos.DrawLine(new Vector3(last.X + 0.5f, last.Y + 0.5f, 0), new Vector3(current.X + 0.5f, current.Y + 0.5f, 0));
        }
    }
}
