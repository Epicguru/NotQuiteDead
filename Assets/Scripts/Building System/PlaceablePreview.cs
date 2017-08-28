using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class PlaceablePreview : NetworkBehaviour
{
    public GameObject Ghost;
    public Color colour = new Color(0, 1, 0, 0.4f);

    private Placeable placeable;
    private float rotation;
    private static Vector3 scale = new Vector3();

    public void Start()
    {

        placeable = GetComponent<Placeable>();

        if(Ghost == null)
        {
            Debug.LogError("No ghost object/renderer assigned!");
            Debug.Break();
            return;
        }

        foreach(SpriteRenderer r in Ghost.GetComponentsInChildren<SpriteRenderer>())
        {
            r.color = colour;
        }
    }

    public void Update()
    {
        if(!placeable.IsPlaced && placeable.Item.IsEquipped() && hasAuthority)
        {
            // Show ghost...
            Ghost.SetActive(true);
            float x = 1f / placeable.ItemScale.x;
            float y = 1f / placeable.ItemScale.y;
            scale.Set(x, y, 1);
            Ghost.transform.localScale = scale;

            Ghost.gameObject.transform.position = InputManager.GetMousePos();
            Ghost.gameObject.transform.rotation = Quaternion.Euler(0, 0, rotation);

            // Rotate...
            // TODO add inputs...
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
        else
        {
            Ghost.SetActive(false);
        }
    }

    public Vector3 GetPosition()
    {
        Debug.Log("Preview is at " + Ghost.transform.position);
        return InputManager.GetMousePos();
    }

    public Quaternion GetRotation()
    {
        return Ghost.transform.rotation;
    }
}
