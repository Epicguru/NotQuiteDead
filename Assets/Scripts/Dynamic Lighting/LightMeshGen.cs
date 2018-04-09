using UnityEngine;

public class LightMeshGen : MonoBehaviour
{
    public MeshFilter Filter;

    public int Width;
    public int Height;
    public float Size = 1;

    public Vector3[] Vertices;

    public void GenMesh()
    {
        int numTiles = Width * Height;
        int numTris = numTiles * 2;

        int vsize_x = Width + 1;
        int vsize_z = Height + 1;
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
                vertices[y * vsize_x + x] = new Vector3(x * Size, y * Size, 0);
                normals[y * vsize_x + x] = -Vector3.forward;
                uv[y * vsize_x + x] = new Vector2((float)x / Width, 1f - (float)y / Height);
            }
        }

        for (y = 0; y < Height; y++)
        {
            for (x = 0; x < Width; x++)
            {
                int squareIndex = y * Width + x;
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

        this.Vertices = vertices;

        Filter.mesh = mesh;
    }
}