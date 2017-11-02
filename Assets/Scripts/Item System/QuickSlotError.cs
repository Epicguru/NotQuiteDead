using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuickSlotError : MonoBehaviour
{
    public string Message
    {
        get
        {
            return _Message;
        }
        set
        {
            _Message = value;
            TimeSinceStart = 0f;
        }
    }

    private string _Message = null;

    public Color Colour = new Color();
    public float FadeTime = 1f;
    public AnimationCurve FadeCurve;
    public float TimeSinceStart = 100f;
    public Text Text;

    public void Update()
    {
        TimeSinceStart += Time.deltaTime;
        float p = TimeSinceStart / FadeTime;

        Color final = Colour;

        // Larger p, more faded.
        if(p >= 1f)
        {
            final = Color.clear;
        }
        else
        {
            final = Color.LerpUnclamped(Colour, Color.clear, FadeCurve.Evaluate(p));
        }

        Text.text = Message;
        Text.color = final;
    }
}