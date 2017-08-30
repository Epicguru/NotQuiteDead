using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrosshairPart : MonoBehaviour {

    public float distance;
    public bool isX;
    public float multi;

    private static Vector2 pos = new Vector2();
    public void Update()
    {
        RectTransform r = (transform as RectTransform);

        if (isX)
        {
            pos.Set(distance * multi, 0);
        }
        else
        {
            pos.Set(0, distance * multi);
        }

        r.anchoredPosition = pos;
    }
}
