using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Crosshair : MonoBehaviour {

    public float MinDistance;
    public float MaxDistance;
    public float Distance;
    public AnimationCurve Curve;
    public bool Active = true;

    private Image[] images;
    private CrosshairPart[] parts;

    public static Crosshair Instance;

    public void Start()
    {
        images = GetComponentsInChildren<Image>();
        parts = GetComponentsInChildren<CrosshairPart>();
        Instance = this;
    }

    public void Update()
    {
        Cursor.visible = !Active || UI.AnyOpen;

        foreach (Image i in images)
        {
            i.enabled = Active && !UI.AnyOpen;
        }

        if (!Active || UI.AnyOpen)
            return;

        (transform as RectTransform).anchoredPosition = Input.mousePosition;
        float dst = Curve.Evaluate(Distance) * MaxDistance + MinDistance;

        foreach (CrosshairPart part in parts)
        {
            part.distance = dst;
        }
    }
}
