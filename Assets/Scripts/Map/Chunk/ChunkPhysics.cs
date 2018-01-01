using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkPhysics : MonoBehaviour {

    public Chunk Chunk;
    public CompositeCollider2D Composite;
    public bool Enabled = true;

    public Collider2D[][] Colliders;

    public void BuildMap()
    {
        Colliders = new Collider2D[Chunk.Width][];
        for (int x = 0; x < Chunk.Width; x++)
        {
            Colliders[x] = new Collider2D[Chunk.Height];
        }
    }

    public void AssignCollider(Collider2D prefab, int x, int y)
    {
        if (!Enabled)
            return;


        if(!Chunk.InBounds(x, y))
        {
            Debug.LogError("Tile outside of chunk bounds! Cannot set physics body.");
            return;
        }

        if(Colliders[x][y] != null)
        {
            Destroy(Colliders[x][y].gameObject);
        }

        if(prefab == null)
        {
            Colliders[x][y] = null;
            return;
        }

        GameObject instance = Instantiate(prefab.gameObject, transform);
        Collider2D colliderInstance = instance.GetComponent<Collider2D>();

        instance.transform.localPosition = new Vector3((x * Chunk.Mesh.TileSize) + (Chunk.Mesh.TileSize * 0.5f), (y * Chunk.Mesh.TileSize) + (Chunk.Mesh.TileSize * 0.5f), 0);

        instance.name = instance.name.Replace("(Clone)", "");
        instance.name = instance.name + " (" + x + "," + y + ")";

        if(colliderInstance == null)
        {
            Debug.LogError("There is not a collider2D on the prefab supplied! (On the instance created from it).");
            return;
        }

        Colliders[x][y] = colliderInstance;
    }

    public void RemoveCollider(int x, int y)
    {
        if (!Enabled)
            return;

        if (!ColliderExists(x, y))
            return;

        Collider2D c = Colliders[x][y];
        Colliders[x][y] = null;

        Destroy(c.gameObject);
    }

    public bool ColliderExists(int x, int y)
    {
        return Colliders[x][y] != null;
    }

    public void DeactivateComposite()
    {
        Composite.enabled = false;
    }

    public void ActivateComposite()
    {
        Composite.enabled = true;
    }
}