using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeleeWeapon))]
public class MeleeAttack : MonoBehaviour {

    public Collider2D Collider;

    private MeleeWeapon weapon;
    private List<Collider2D> touching;

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
    
    public void OnCollisionEnter2D(Collision2D c)
    {
        Debug.Log(c.collider);
    }
}
