using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZoom : MonoBehaviour {

    public float Size
    {
        get
        {
            return _Size;
        }
        set
        {
            _Size = Mathf.Clamp(value, 0.1f, 100f);
        }
    }
    [Range(0f, 1f)]
    public float Speed;

    public static CameraZoom I;

    private float _Size;
    private Camera cam;

    public void Start()
    {
        I = this;
        cam = GetComponent<Camera>();

        InvokeRepeating("Tick", 0, 1f / 60f);
    }

    public void Update()
    {
        Size -= (Input.mouseScrollDelta).y;
    }

    public void Tick()
    {
        cam.orthographicSize += (Size - cam.orthographicSize) * Speed;
    }
}
