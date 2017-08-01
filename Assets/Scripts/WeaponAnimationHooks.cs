using UnityEngine;
using System.Collections;

public class WeaponAnimationHooks : MonoBehaviour
{
    private Weapon w;
    private void Start()
    {
        w = GetComponentInParent<Weapon>();
    }

    public void Chamber()
    {
        w.Anim_Chamber();
    }

    public void Reload()
    {
        w.Anim_Reload();
    }

    public void Shoot()
    {
        w.Anim_Shoot();
    }

    public void PlaySound(int index)
    {
        w.Anim_Sound(index);
    }

    public void SpawnMagazine()
    {
        w.Anim_SpawnMag();
    }

    public void SpawnShell()
    {
        w.Anim_SpawnShell();
    }
}
