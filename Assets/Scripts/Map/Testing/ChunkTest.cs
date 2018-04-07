using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkTest : MonoBehaviour
{
    public string[] BGs;

    public void Start()
    {
        GetComponent<ChunkBackground>().BG = Backgrounds.Loaded[BGs[Random.Range(0, BGs.Length)]];
        GetComponent<ChunkBackground>().Regenerate();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GetComponent<ChunkBackground>().Regenerate();
        }
    }
}