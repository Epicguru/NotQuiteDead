using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeleeWeapon))]
public class MeleeAttack : MonoBehaviour {

    public Collider2D Collider;

    private MeleeWeapon weapon;

    public void Start()
    {
        weapon = GetComponent<MeleeWeapon>();
        if(Collider == null)
        {
            Debug.LogError("Collider for attacking with " + weapon.Item.Name + " is null!");
            Debug.Break();
            return;
        }
    }   
    
    public Collider2D[] GetHitColliders()
    {
        Collider2D[] list;
        Collider.GetContacts(list = new Collider2D[0]);

        return list;
    } 

    public void DebugHits()
    {
        foreach(Collider2D c in GetHitColliders())
        {
            Debug.Log(c);
        }
    }
}
