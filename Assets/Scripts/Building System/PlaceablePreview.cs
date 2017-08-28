using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class PlaceablePreview : NetworkBehaviour
{
    public GameObject Preview;
    public Color colour = new Color(0, 1, 0, 0.4f);   

    private Placeable placeable;
    private float rotation;
    private static Vector3 scale = new Vector3();

    public void Start()
    {
        placeable = GetComponent<Placeable>();

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
            Preview.transform.localScale = Vector3.one;
            Preview.transform.position = InputManager.GetMousePos();
            Preview.transform.rotation = Quaternion.Euler(0, 0, rotation);

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
            Preview.SetActive(false);
        }
    }

    public void OnDestroy()
    {
        Destroy(Preview.gameObject);
    }

    public Vector3 GetPosition()
    {
        Debug.Log("Preview is at " + Preview.transform.position);
        return InputManager.GetMousePos();
    }

    public Quaternion GetRotation()
    {
        return Preview.transform.rotation;
    }
}
