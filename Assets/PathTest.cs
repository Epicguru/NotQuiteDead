using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathTest : MonoBehaviour {

    public Transform A, B;
    public bool Line;
    public Color LineColour;
    public TileLayer Layer;
    public List<Node> path;


    private Vector3 oldPosA;
    private Vector3 oldPosB;

    public void Update()
    {
        if(B.transform.position != oldPosB || A.transform.position != oldPosA)
            Pathfinding.Find((int)A.transform.position.x, (int)A.transform.position.y, (int)B.transform.position.x, (int)B.transform.position.y, Layer, PathFound);

        oldPosA = A.transform.position;
        oldPosB = B.transform.position;
    }

    public void PathFound(List<Node> path)
    {
        this.path = path;
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
