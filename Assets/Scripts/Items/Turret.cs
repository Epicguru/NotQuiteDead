using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour {

    public Transform Target;
    public float MaxRange;

    public SpriteRenderer Colour;
    public Transform BarrelRotation;
    public Transform Rotation;

    private Placeable placeable;
    private float speed;

    public void Start()
    {
        placeable = GetComponent<Placeable>();
    }

    public void Update()
    {
        bool aiming = placeable.IsPlaced && Target != null && InRange();

        if (aiming)
        {
            float angle = CalculateAngle();

            Rotation.transform.rotation = Quaternion.Euler(0, 0, angle - 90);

            speed = 3000f;
            BarrelRotation.transform.Rotate(0, speed * Time.deltaTime, 0);
        }
        else
        {
            if(Rotation.localRotation.z > 0.5f || Rotation.localRotation.z < -0.5f)
            {
                if (Rotation.localRotation.z > 0)
                {
                    Rotation.Rotate(0, 0, -90f * Time.deltaTime);
                }
                if (Rotation.localRotation.z < 0)
                {
                    Rotation.Rotate(0, 0, 90f * Time.deltaTime);
                }
                speed -= Time.deltaTime * (speed * 0.1f);
                BarrelRotation.transform.Rotate(0, speed * Time.deltaTime, 0);

            }
        }
    } 
    
    public bool InRange()
    {
        if (Target == null)
            return false;

        return Vector2.Distance(Target.position, transform.position) <= MaxRange;
    }

    public float CalculateAngle()
    {
        if (Target == null)
            return 0f;

        float dstX = Target.position.x - transform.position.x;
        float dstY = Target.position.y - transform.position.y;

        float angle = Mathf.Atan2(dstY, dstX) * Mathf.Rad2Deg;

        return angle;
    }
}
