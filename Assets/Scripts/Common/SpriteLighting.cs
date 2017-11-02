using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class SpriteLighting : MonoBehaviour
{
    public SpriteRenderer[] Exceptions;
    public bool Shadows
    {
        get
        {
            return _Shadows;
        }
        set
        {
            if(value != _Shadows)
            {
                // Shadows are changing...
                foreach (MeshRenderer r in this.GetComponentsInChildren<MeshRenderer>(true))
                {
                    r.enabled = value;
                }
            }
            _Shadows = value;
        }
    }
    private bool _Shadows = true;
    [NonSerialized] public float YRange = 1000f;
    [NonSerialized] public float MinZ = 0, MaxZ = 0.1f;

    public void Start()
    {
        foreach (SpriteRenderer r in GetComponentsInChildren<SpriteRenderer>(true))
        {
            if (Exceptions.Contains<SpriteRenderer>(r))
                continue;
            r.material = MaterialProvider._Instance.LitSprite;
            r.UpdateGIMaterials();
        }
    }

    public float CalculateZ()
    {
        float y = transform.position.y + (transform.position.x / 2f); // Use X as a sort of tiebreaker, but mainly y.
        float p = (Mathf.Abs(y) / YRange);
        float z = Mathf.Lerp(MinZ, MaxZ, p);

        return z;
    }

    private static Vector3 position = new Vector3();
    public void Update()
    {
        float z = CalculateZ();

        position.x = transform.position.x;
        position.y = transform.position.y;
        position.z = z;

        transform.position = position;
    }
}
