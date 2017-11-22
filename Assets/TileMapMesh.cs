using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileMapMesh : MonoBehaviour {

    public MeshFilter Mesh;

    public Vector2 Bounds = Vector2.one;

    public void Update()
    {
        Gen();
    }

    public void Gen()
    {
        Mesh Mesh = this.Mesh.mesh;

        Vector3[] vertices = new Vector3[4];

        vertices[0] = new Vector3(0, 0, 0);
        vertices[1] = new Vector3(Bounds.x, 0, 0);
        vertices[2] = new Vector3(0, Bounds.y, 0);
        vertices[3] = new Vector3(Bounds.x, Bounds.y, 0);

        Mesh.vertices = vertices;

        int[] triangles = new int[6];

        triangles[0] = 0;
        triangles[1] = 2;
        triangles[2] = 1;

        triangles[3] = 2;
        triangles[4] = 3;
        triangles[5] = 1;

        Mesh.triangles = triangles;

       Vector2[] uv = new Vector2[4];

        uv[0] = new Vector2(0, 0);
        uv[1] = new Vector2(1, 0);
        uv[2] = new Vector2(0, 1);
        uv[3] = new Vector2(1, 1);

        Mesh.uv = uv;

        Vector3[] normals = new Vector3[4];

        normals[0] = -Vector3.forward;
        normals[1] = -Vector3.forward;
        normals[2] = -Vector3.forward;
        normals[3] = -Vector3.forward;

        Mesh.normals = normals;
    }
}
