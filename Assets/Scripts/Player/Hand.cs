using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour {

    // On the player hand to apply stuff like skin colour.

    // Does nothing for now.

    public bool Render = true;

    private SpriteRenderer r;

    public static void SetEquipHandRender(Transform parent, bool flag)
    {
        foreach(Hand hand in parent.GetComponentsInChildren<Hand>())
        {
            hand.Render = flag;
        }
    }

    public void Start()
    {
        r = GetComponent<SpriteRenderer>();
    }

    public void Update()
    {
        r.enabled = Render;
    }
}
