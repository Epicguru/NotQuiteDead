﻿using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class DebugText : MonoBehaviour {

    public static DebugText _Instance;

    public Image Box;

    public bool Active;

    public string str;

    public Text Text;

    public int FPS
    {
        get
        {
            return fps;
        }
        private set
        {
            fps = value;
        }
    }
    private int fps;

    private int frames;
    private float timer;

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
        _Instance.str += text + '\n';
    }

    public static void Log(string text, Color colour)
    {
        if (!_Instance.Active)
            return;
        _Instance.str += RichText.InColour(text, colour) + '\n';
    }

    public void Update()
    {
        if(InputManager.InputDown("Debug View"))
        {
            Active = !Active;
        }

        Box.enabled = Active;
        Text.enabled = Active;

        frames++;
        timer += Time.unscaledDeltaTime;
        if(timer >= 1f)
        {
            // I know the while loop inside the if looks odd, but I have a reason...
            // I just wont tell you it :D
            while(timer >= 1f)
            {
                timer -= 1f;
            }

            FPS = frames;
            frames = 0;
        }
    }

    public void LateUpdate()
    {
        if (!Active)
            return;

        str = (RichText.InBold(RichText.InColour("FPS: " + FPS, FPS > 55 ? Color.green : FPS > 30 ? Color.yellow : Color.red)) + "\n") + str;
        Text.text = str;
        str = "";
    }
}