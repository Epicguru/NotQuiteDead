using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathTest : MonoBehaviour {

    public Transform A, B;
    public Color LineColour;

    private List<Node> path;

    private Vector3 oldPosA;
    private Vector3 oldPosB;

    public void Update()
    {
        if(B.transform.position != oldPosB || A.transform.position != oldPosA)
            Pathfinding.Find("Path Test", (int)A.transform.position.x, (int)A.transform.position.y, (int)B.transform.position.x, (int)B.transform.position.y, World.Instance.TileMap.GetLayer("Foreground"), PathFound);

        oldPosA = A.transform.position;
        oldPosB = B.transform.position;
    }

    public void PathFound(List<Node> path)
    {
        this.path = path;
    }
         
    public void OnDrawGizmos()
    {
        Pathfinding.DrawPathLine(path, LineColour);
    }
}
