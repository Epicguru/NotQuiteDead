﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkTest : MonoBehaviour {

    public string[] BGs;

    public void Start()
    {
        GetComponent<ChunkBackground>().BG = Backgrounds.Loaded[BGs[Random.Range(0, BGs.Length)]];
    }
}
