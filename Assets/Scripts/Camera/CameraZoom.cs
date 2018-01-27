using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZoom : MonoBehaviour {

    public Camera[] Cams;

    public float MaxSize = 10f;
    public float Size
    {
        get
        {
            return _Size;
        }
        set
        {
            _Size = Mathf.Clamp(value, 1f, MaxSize);
        }
    }
    [Range(0f, 1f)]
    public float Speed;

    public static CameraZoom I;

    private float _Size = 5;
    private float timer;
    private float value;

    public void Start()
    {
        I = this;
    }

    public void Update()
    {
        if (!UI.AnyOpen)
            Size -= (Input.mouseScrollDelta).y * (Size / 10f);

        timer += Time.unscaledDeltaTime;
        while(timer >= 1f / 60f)
        {
            timer -= 1f / 60f;
            Tick();
        }
    }

    public void Tick()
    {
        foreach(var cam in Cams)
        {
            cam.orthographicSize += (Size - cam.orthographicSize) * Speed;
        }
    }
}
