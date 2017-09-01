using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZoom : MonoBehaviour {

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
    private Camera cam;

    public void Start()
    {
        I = this;
        cam = GetComponent<Camera>();

        InvokeRepeating("Tick", 0, 1f / 60f);
    }

    public void Update()
    {
        Size -= (Input.mouseScrollDelta).y * (Size / 10f);
    }

    public void Tick()
    {
        cam.orthographicSize += (Size - cam.orthographicSize) * Speed;
    }
}
