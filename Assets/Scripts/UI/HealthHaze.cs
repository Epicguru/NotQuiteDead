using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthHaze : MonoBehaviour {

    private Image Image;

    public void Start()
    {
        Image = GetComponent<Image>();
    }

    private Color colour = new Color();
    public void Update()
    {
        if (Player.Local == null)
            return;

        colour.r = Image.color.r;
        colour.g = Image.color.g;
        colour.b = Image.color.b;
        colour.a = 1f - Player.Local.Health.GetHealthPercentage();

        Image.color = colour;
    }
}
