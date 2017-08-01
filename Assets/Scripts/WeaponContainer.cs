using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponContainer : MonoBehaviour {

    private Weapon w;
    private Animator a;

    public void Start()
    {
        w = GetComponentInChildren<Weapon>();
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
            GameObject.Find("Player").GetComponentInChildren<WeaponManager>().Equip(this.gameObject, -1);
        }
    }
}