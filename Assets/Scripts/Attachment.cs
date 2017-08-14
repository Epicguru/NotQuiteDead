using UnityEngine;
using System.Collections;

public class Attachment : MonoBehaviour
{
    public string Prefab = "Weapons/Attatchments/";
    public string Name = "Default Name";
    public AttachmentType Type;
    [HideInInspector] public Gun AttatchedWeapon;

    public GameObject GetPrefab()
    {
        return Resources.Load<GameObject>(Prefab);
    }

    public override bool Equals(object other)
    {
        if (!(other is Attachment))
            return false;
        return (other as Attachment).Prefab == Prefab;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}
