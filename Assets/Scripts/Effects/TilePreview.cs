using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilePreview : MonoBehaviour
{
    [Header("Colours")]
    public Color YesColour;
    public Color NoColour;

    [Header("Controls")]
    public bool CanPlace = true;

    [Header("Pulse")]
    public float BaseAlpha;
    public float Amplitude;
    public float Frequency;

    private SpriteRenderer Renderer;
    private float timer;

    public void Awake()
    {
        Renderer = GetComponentInChildren<SpriteRenderer>();
    }

    public void Update()
    {
        timer += Time.unscaledDeltaTime;

        float alpha = BaseAlpha + (Mathf.Sin(timer * Frequency) * Amplitude);

        Renderer.color = MakeColour(CanPlace ? YesColour : NoColour, alpha);
    }

    public Color MakeColour(Color col, float alpha)
    {
        return new Color(col.r, col.g, col.b, alpha);
    }
}