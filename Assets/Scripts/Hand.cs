﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Hand : MonoBehaviour
{
    private new SpriteRenderer renderer;
    private Gun w;

    public void Start()
    {
        renderer = GetComponent<SpriteRenderer>();
        w = GetComponentInParent<Gun>();
    }

    public void Update()
    {
        if(w.Stored || w.Dropped)
        {
            renderer.enabled = false;
        }
        else
        {
            renderer.enabled = true;
        }
    }

}
