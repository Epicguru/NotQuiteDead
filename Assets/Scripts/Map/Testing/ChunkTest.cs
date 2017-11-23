using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkTest : MonoBehaviour {

    public BoxCollider2D ColliderPrefab;

    public Sprite[] Sprites;

    public void Start()
    {
        Chunk chunk = GetComponent<Chunk>();
        for (int i = 0; i < Sprites.Length; i++)
        {
            chunk.Texture.SetTile(Sprites[i], i, i);
        }

        chunk.Physics.AssignCollider(ColliderPrefab, 1, 1);
        chunk.Physics.AssignCollider(ColliderPrefab, 0, 0);
        chunk.Physics.AssignCollider(ColliderPrefab, 4, 0);
    }
}
