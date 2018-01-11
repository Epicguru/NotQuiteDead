using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class DebugText : MonoBehaviour {

    public static DebugText _Instance;

    public bool Active;

    private StringBuilder sB = new StringBuilder();
    public Text Text;

    public void Awake()
    {
        _Instance = this;
    }

    public static void Log(string text)
    {
        if (!_Instance.Active)
            return;
        _Instance.sB.Append(text);
        _Instance.sB.Append('\n');
    }

    public static void Log(string text, Color colour)
    {
        if (!_Instance.Active)
            return;
        if(_Instance.sB.Length != 0)
            _Instance.sB.Append('\n');
        _Instance.sB.Append(RichText.InColour(text, colour));
    }

    public void Update()
    {
        if(InputManager.InputDown("Debug View"))
        {
            Active = !Active;
        }

        Text.enabled = Active;
    }

    public void LateUpdate()
    {
        if (!Active)
            return;

        string text = sB.ToString();

        sB.Length = 0;

        Text.text = text;
    }
}