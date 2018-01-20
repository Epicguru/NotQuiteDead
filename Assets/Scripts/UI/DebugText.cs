using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class DebugText : MonoBehaviour {

    public static DebugText _Instance;

    public Image Box;

    public bool Active;

    private StringBuilder sB = new StringBuilder();
    public Text Text;

    public void Awake()
    {
        _Instance = this;
    }

    public void OnDestroy()
    {
        if(_Instance == this)
        {
            _Instance = null;
        }
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
        _Instance.sB.Append(RichText.InColour(text, colour));
        _Instance.sB.Append('\n');
    }

    public void Update()
    {
        if(InputManager.InputDown("Debug View"))
        {
            Active = !Active;
        }

        Box.enabled = Active;

        Text.enabled = Active;
    }

    public void LateUpdate()
    {
        if (!Active)
            return;

        string text = sB.ToString().Trim();
        sB.Length = 0;

        Text.text = text;
    }
}