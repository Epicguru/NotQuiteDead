using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGen : MonoBehaviour
{
    [Tooltip("Mesh size, in tiles.")]
    public Vector2Int MeshSize;

    [Tooltip("Tile Size, in Units.")]
    public float TileSize = 1f;

    public MeshFilter Filter;

    public void Start()
    {
        GenMesh();
    }

    public void GenMesh()
    {
        int numTiles = MeshSize.x * MeshSize.y;
        int numTris = numTiles * 2;

        int vsize_x = MeshSize.x + 1;
        int vsize_z = MeshSize.y + 1;
        int numVerts = vsize_x * vsize_z;

        // Generate the mesh data
        Vector3[] vertices = new Vector3[numVerts];
        Vector3[] normals = new Vector3[numVerts];
        Vector2[] uv = new Vector2[numVerts];

        int[] triangles = new int[numTris * 3];

        int x, y;
        for (y = 0; y < vsize_z; y++)
        {
            for (x = 0; x < vsize_x; x++)
            {
                vertices[y * vsize_x + x] = new Vector3(x * TileSize, y * TileSize, 0);
                normals[y * vsize_x + x] = -Vector3.forward;
                uv[y * vsize_x + x] = new Vector2((float)x / MeshSize.x, 1f - (float)y / MeshSize.y);
            }
        }

        for (y = 0; y < MeshSize.y; y++)
        {
            for (x = 0; x < MeshSize.x; x++)
            {
                int squareIndex = y * MeshSize.x + x;
                int triOffset = squareIndex * 6;
                triangles[triOffset + 1] = y * vsize_x + x + 0;
                triangles[triOffset + 2] = y * vsize_x + x + vsize_x + 0;
                triangles[triOffset + 0] = y * vsize_x + x + vsize_x + 1;

                triangles[triOffset + 4] = y * vsize_x + x + 0;
                triangles[triOffset + 5] = y * vsize_x + x + vsize_x + 1;
                triangles[triOffset + 3] = y * vsize_x + x + 1;
            }
        }

        // Create a new Mesh and populate with the data
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.normals = normals;
        mesh.uv = uv;

        Filter.mesh = mesh;
    }
}