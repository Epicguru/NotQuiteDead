using UnityEngine;
using System.Collections;

public class GunAnimationHooks : MonoBehaviour
{
    private Gun g;
    private void Start()
    {
        g = GetComponentInParent<Gun>();
    }

    public void Chamber()
    {
        g.Anim_Chamber();
    }

    public void Reload()
    {
        g.Anim_Reload();
    }

    public void Shoot()
    {
        g.Anim_Shoot();
    }

    public void PlaySound(int index)
    {
        g.Anim_Sound(index);
    }

    public void SpawnMagazine()
    {
        g.Anim_SpawnMag();
    }

    public void SpawnShell()
    {
        g.Anim_SpawnShell();
    }
}
