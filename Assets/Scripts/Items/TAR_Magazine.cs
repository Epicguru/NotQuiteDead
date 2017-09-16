using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TAR_Magazine : MonoBehaviour
{
    [Range(0f, 1f)]
    public float Percentage = 0f;
    public Animator Anim;
    public GunShooting Shooting;
    public bool SetPercentage = true;

    public void Start()
    {
        Anim = GetComponent<Animator>();
        Shooting = GetComponentInParent<GunShooting>();
    }

    public void Update()
    {
        if(SetPercentage)
            Percentage = 1f - Shooting.bulletsInMagazine / (float)Shooting.Capacity.MagazineCapacity;
        Anim.Play("Mag", 0, Mathf.Clamp(Percentage, 0f, 0.999f));
    }
}