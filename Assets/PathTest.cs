using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathTest : MonoBehaviour {

    public Transform A, B;
    public bool Line;
    public bool NewCost;
    public Color LineColour;

    public TileLayer Layer;

    public List<Node> path;

    public void Update()
    {
        path = Pathfinding.Find((int)A.transform.position.x, (int)A.transform.position.y, (int)B.transform.position.x, (int)B.transform.position.y, Layer);
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = LineColour;

        if(path == null)
        {
            return;
        }

        if (Line)
        {
            for (int i = 0; i < path.Count; i++)
            {
                Node last = null;
                if (i != 0)
                {
                    last = path[i - 1];
                }
                Node current = path[i];

                if (last != null)
                    Gizmos.DrawLine(new Vector3(last.X + 0.5f, last.Y + 0.5f, 0), new Vector3(current.X + 0.5f, current.Y + 0.5f, 0));
            }
        }
        else
        {
            for (int i = 0; i < path.Count; i++)
            {
                Gizmos.DrawIcon(new Vector3(path[i].X + 0.5f, path[i].Y + 0.5f, 0), "Pathmarker.png", true);
            }
        }
    }
}
