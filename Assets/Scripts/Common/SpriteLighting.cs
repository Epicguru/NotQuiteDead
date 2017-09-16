using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class SpriteLighting : MonoBehaviour
{
    public SpriteRenderer[] Exceptions;

    public void Start()
    {
        foreach(SpriteRenderer r in GetComponentsInChildren<SpriteRenderer>(true))
        {
            if (Exceptions.Contains<SpriteRenderer>(r))
                continue;
            r.material = TileMeshes.Instance.LitSprite;
        }
    }
}
