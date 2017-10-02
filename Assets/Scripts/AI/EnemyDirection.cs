using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class EnemyDirection : NetworkBehaviour
{
    public bool Right = true;

    private static Vector3 scale = new Vector3(1, 1, 1);
    public void Update()
    {
        scale.x = Right ? 1f : -1f;
        transform.localScale = scale;
    }
}
