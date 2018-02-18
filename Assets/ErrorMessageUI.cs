using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ErrorMessageUI : MonoBehaviour
{
    public static ErrorMessageUI Instance;

    public string DisplayMessage
    {
        get
        {
            return _DM;
        }
        set
        {
            _DM = value;
            time = 0f;
        }
    }
    private string _DM;

    [Header("Display Time")]
    public float Duration = 2f;
    public AnimationCurve AlphaCurve = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(0.7f, 1f), new Keyframe(1f, 0f));

    [Header("Pulse")]
    public float PulseMagnitude;
    public float PulseBase;
    public float PulseFrequency;

    [SerializeField]
    private Text Text;
    [SerializeField]
    private float time;

    public void Awake()
    {
        time = Duration + 1;
        Instance = this;
    }

    public void OnDestroy()
    {
        Instance = null;
    }

    public void Update()
    {
        time += Time.unscaledDeltaTime;
        float alpha = CalculateAlpha();

        Text.text = DisplayMessage;
        Text.color = GetColour(Text.color, alpha);
    }

    public Color GetColour(Color c, float a)
    {
        return new Color(c.r, c.g, c.b, a);
    }

    public float CalculateAlpha()
    {
        float p = Mathf.Clamp(time / Duration, 0f, 1f);
        float x = Mathf.Clamp(AlphaCurve.Evaluate(p), 0f, 1f);

        return GetPulseAlpha() * x;
    }

    public float GetPulseAlpha()
    {
        return PulseBase + (Mathf.Sin(time * PulseFrequency) * PulseMagnitude);
    }
}
