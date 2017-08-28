using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuzzleFlash : MonoBehaviour {

    // Is static

    public Transform Prefab;
    public Sprite[] sprites;

    public static MuzzleFlash Instance;

    public void Start()
    {
        Instance = this;
    }

    public static GameObject Place(Vector3 position, Quaternion rotation, Transform parent = null)
    {
        GameObject GO = Instantiate(Instance.Prefab.gameObject, position, rotation, parent);
        GO.GetComponentInChildren<SpriteRenderer>().sprite = Instance.sprites[Random.Range(0, Instance.sprites.Length)];

        return GO;
    }
}
