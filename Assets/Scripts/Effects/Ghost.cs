using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Ghost : MonoBehaviour
{
    public float Time;
    public AnimationCurve Fade = AnimationCurve.EaseInOut(0, 1, 1, 0);
    public PoolType PoolType;

    private float RealTime;
    private SpriteRenderer[] renderers;

    public void Start()
    {
        RealTime = Time;
        renderers = GetComponentsInChildren<SpriteRenderer>();
    }

    private static Color colour = new Color();
    public void Update()
    {
        RealTime -= UnityEngine.Time.deltaTime;
        float p = RealTime / Time;
        float value = Fade.Evaluate(1f - p);

        if (p <= 0)
        {
            ObjectPool.Destroy(gameObject, PoolType);
            return;
        }

        foreach(SpriteRenderer r in renderers)
        {
            colour.r = r.color.r;
            colour.g = r.color.g;
            colour.b = r.color.b;
            colour.a = value;
            r.color = colour;
        }
    }
}
