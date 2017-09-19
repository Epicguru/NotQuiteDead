using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretLaserLength : MonoBehaviour {

    public RaycastHit2D[] hits;
    private Turret Turret;

    public void Start()
    {
        Turret = GetComponentInParent<Turret>();
    }

    public void Update()
    {
        if (Turret.Placeable != null && !Turret.Placeable.IsPlaced)
        {
            SetLength(0);
            return;
        }

        // Raycast...
        hits = Physics2D.RaycastAll(transform.position, transform.up, Turret.MaxRange);
        Debug.DrawRay(transform.position, transform.up * Turret.MaxRange, Color.red);
        foreach(RaycastHit2D hit in hits)
        {
            //Debug.Log("Hit on " + hit.transform.name);
            if (hit.transform.GetComponentInParent<Turret>() == Turret)
            {
                //Debug.Log("Is turret!");
                continue;
            }
            if (hit.collider.isTrigger == false) // Solid
            {
                SetLength(Vector2.Distance(transform.position, hit.point));
                //Debug.Log("Set length to solid!");
                return;
            }
            if (hit.collider.isTrigger && hit.transform.GetComponentInParent<Health>() != null)
            {
                if(hit.transform.GetComponentInParent<Health>().CannotHit.Contains(hit.collider))
                {
                    //Debug.Log("Has health and cannot hit.");
                    continue;
                }
                else
                {
                    //Debug.Log("Has health and can hit!");
                    SetLength(Vector2.Distance(transform.position, hit.point));
                    return;
                }
            }
        }
        SetLength(Turret.MaxRange);
    }

    private Vector3 vector = new Vector3();
    public void SetLength(float length)
    {
        vector.Set(0.5f, length, 1f);
        transform.localScale = vector;
    }
}
