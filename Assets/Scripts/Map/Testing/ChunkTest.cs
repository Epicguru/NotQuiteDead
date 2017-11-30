using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkTest : MonoBehaviour {

    public BoxCollider2D ColliderPrefab;

    public Sprite[] Sprites;

    public void Start()
    {
        Chunk chunk = GetComponent<Chunk>();

        chunk.Texture.SetTiles(Sprites[Random.Range(0, Sprites.Length)], 0, 0, chunk.Width, chunk.Height);

        chunk.Physics.AssignCollider(ColliderPrefab, 1, 1);
        chunk.Physics.AssignCollider(ColliderPrefab, 0, 0);
        chunk.Physics.AssignCollider(ColliderPrefab, 4, 0);
    }
}
