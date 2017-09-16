using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class KillFeedPanel : MonoBehaviour
{
    public float Height = 30f;
    public float Padding = 10f;
    public RectTransform x, y, z;

    public void Update()
    {
        if (x == null || y == null || z == null)
            return;

        (transform as RectTransform).sizeDelta = new Vector2(GetWidth(), Height);
    }

    public float GetWidth()
    {
        return x.sizeDelta.x + y.sizeDelta.x + z.sizeDelta.x + Padding;
    }
}
