using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public string Prefab;
    public string Name;

    public bool IsSolid = false;

    public Sprite sprite;

    private MeshFilter filter;
    private MeshRenderer mesh;

    private int x, y;
    private TiledMap map;

    public void Start()
    {
        filter = GetComponentInChildren<MeshFilter>(true);
        mesh   = GetComponentInChildren<MeshRenderer>(true);

        transform.position = new Vector3(transform.position.x, transform.position.y, 2);

        GetComponentsInChildren<Transform>()[1].transform.rotation = Quaternion.Euler(-90, 0, 0);     // TODO fixme!
        GetComponentsInChildren<Transform>()[1].transform.localPosition = new Vector3(0, 0, IsSolid ? -0.5f : 0);    // TODO fixme!

        if (IsSolid)
        {
            transform.localScale = new Vector3(1, 1, 1);
            filter.mesh = TileMeshes.Instance.Cube;
        }
        else
        {
            transform.localScale = new Vector3(0.1f, 0.1f, 1);
            filter.mesh = TileMeshes.Instance.Plane;
        }

        mesh.sharedMaterial.mainTexture = sprite.texture;
        Rect r = sprite.textureRect;
        mesh.sharedMaterial.mainTextureScale = new Vector2(r.width / (float)sprite.texture.width, r.height / (float)sprite.texture.height);
        mesh.sharedMaterial.mainTextureOffset = new Vector2(r.x / (float)sprite.texture.width, r.y / (float)sprite.texture.height);
    }

    public void SetPosition(TiledMap map, int x, int y)
    {
        transform.position = new Vector2(x, y);
        this.x = x;
        this.y = y;
        this.map = map;
    }

    public Tile GetNeighbour(int x, int y)
    {
        return map.GetTile(this.x + x, this.y + y);
    }

    public bool NeedsBody()
    {
        if (!IsSolid)
            return false;

        Tile above = GetNeighbour(0, 1);
        Tile below = GetNeighbour(0, -1);
        Tile right = GetNeighbour(1, 0);
        Tile left = GetNeighbour(-1, 0);

        if (above == null || !above.IsSolid || below == null || !below.IsSolid || right == null || !right.IsSolid || left == null || !left.IsSolid)
            return true;
        return false;
    }

    public virtual void Ping()
    {
        // Called when the surrounding tiles change.
        UpdatePhysics();
    }

    public void UpdatePhysics()
    {
        if (!IsSolid)
            return;
        if (NeedsBody())
        {
            this.gameObject.AddComponent<BoxCollider2D>();
        }
        else
        {
            Destroy(this.gameObject.GetComponent<BoxCollider2D>());
        }
    }
}
