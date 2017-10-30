﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class PlaceablePreview : NetworkBehaviour
{
    public GameObject Preview;
    public Color colour = new Color(0, 1, 0, 0.4f);
    public static float LastRotation;

    private Placeable placeable;
    private float rotation;

    public void Start()
    {
        placeable = GetComponent<Placeable>();

        rotation = LastRotation;
        if (!placeable.CanRotate)
            rotation = 0;

        if(Preview == null)
        {
            Debug.LogError("No ghost object/renderer assigned!");
            Debug.Break();
            return;
        }

        foreach(SpriteRenderer r in Preview.GetComponentsInChildren<SpriteRenderer>())
        {
            r.color = colour;
        }
    }

    public void Update()
    {
        if (placeable.Item == null)
            return;

        if(!placeable.IsPlaced && placeable.Item.IsEquipped() && hasAuthority)
        {
            // Show ghost...
            Preview.SetActive(true);

            Preview.transform.SetParent(null);
            Preview.transform.localScale = placeable.PlacedScale;
            Preview.transform.position = InputManager.GetMousePos();
            Preview.transform.rotation = Quaternion.Euler(0, 0, rotation);

            // Rotate...
            // TODO add inputs...
            if (placeable.CanRotate)
            {
                float speed = 120f;
                if (Input.GetKey(KeyCode.E))
                {
                    rotation -= speed * Time.deltaTime;
                }
                if (Input.GetKey(KeyCode.Q))
                {
                    rotation += speed * Time.deltaTime;
                }
            }
        }
        else
        {
            Preview.SetActive(false);
        }
    }

    public void OnDestroy()
    {
        Destroy(Preview.gameObject);
    }

    public Vector3 GetPosition()
    {
        return InputManager.GetMousePos();
    }

    public Quaternion GetRotation()
    {
        return Preview.transform.rotation;
    }

    public float GetRotationAngles()
    {
        return rotation;
    }
}
