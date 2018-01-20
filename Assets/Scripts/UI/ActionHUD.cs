using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionHUD : MonoBehaviour {

    public float FadeTime;
    public AnimationCurve Curve;

    public Text Text;
    public CanvasRenderer[] Renderers;

    private float timer;
    private Color tempColour = new Color();
    private static ActionHUD Instance;

    public static void DisplayAction(string s)
    {
        Instance.timer = 0;
        Instance.SetText(s);
    }

    public void Start()
    {
        Instance = this;
        Text = GetComponentInChildren<Text>();
    }

    public void OnDestroy()
    {
        if(Instance == this)
            Instance = null;
    }

    private void SetText(string text)
    {
        if(Text.text != text)
            Text.text = text;
    }

    public void Update()
    {
        timer += Time.unscaledDeltaTime;
        float p = 1 - Mathf.Clamp(timer / FadeTime, 0, 1);

        if(timer < 0.1f)
        {
            p = 1f;
        }

        tempColour.r = 1;
        tempColour.g = 1;
        tempColour.b = 1;
        tempColour.a = Curve.Evaluate(p);

        foreach(var r in this.Renderers)
        {
            r.SetColor(tempColour);
        }
    }
}
