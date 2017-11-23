using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkTest : MonoBehaviour {

    public Sprite[] Sprites;

    public void Start()
    {
        Chunk chunk = GetComponent<Chunk>();
        for (int i = 0; i < Sprites.Length; i++)
        {
            chunk.Texture.SetTile(Sprites[i], i, i);
        }
    }
}
