using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPath : MonoBehaviour {

    public float StartWidth;

    private float time = 0.2f;
    private float startTime;
    private LineRenderer line;

    private static Vector3 temp = new Vector3();
    public void Setup(Vector3 start, Vector2 end)
    {
        time = 0.2f;
        startTime = time;
        if(line == null)
        {
            line = GetComponent<LineRenderer>();
        }
        line.widthMultiplier = StartWidth;

        temp.Set(end.x, end.y, 0);

        line.SetPosition(0, start);
        line.SetPosition(1, Vector3.Lerp(start, temp, 0.5f));
        line.SetPosition(2, end);
    }

    public void Update()
    {
        time -= UnityEngine.Time.deltaTime;
        float p = time / startTime;

        if (p <= 0)
        {
            ObjectPool.Destroy(gameObject, PoolType.BULLET_PATH);
            return;
        }

        line.widthMultiplier = p * StartWidth;
    }
}
