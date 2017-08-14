using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponContainer : MonoBehaviour {

    private Gun w;
    private Animator a;

    public void Start()
    {
        w = GetComponentInChildren<Gun>();
        if(w != null)
            a = w.GetComponentInChildren<Animator>();
    }

    public void Update()
    {
        if (w == null)
            return;
        w.Dropped = true;
        a.SetBool("Dropped", true);
    }

    public void OnMouseOver()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            GameObject o = GameObject.FindGameObjectWithTag("Local Player");
            if(o != null)
                o.GetComponentInChildren<GunManager>().Equip(this.gameObject, -1);
        }
    }
}