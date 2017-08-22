﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class FitChild : MonoBehaviour
{
    public RectTransform[] Targets;
    public bool Width = true, Height = true;

    private static Vector2 tempVector = new Vector2();
    
    public void Update()
    {
        tempVector.Set(!Width ? (transform as RectTransform).sizeDelta.x : 0, !Height ? (transform as RectTransform).sizeDelta.y : 0);

        foreach(RectTransform target in Targets)
        {
            if (Width)
                tempVector.x += target.sizeDelta.x;
            if (Height)
                tempVector.y += target.sizeDelta.y;
        }

        (transform as RectTransform).sizeDelta = tempVector;
    }
}