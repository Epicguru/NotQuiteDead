using UnityEngine;
using System.Collections;

[RequireComponent(typeof(LineRenderer))]
public class VisualBullet : MonoBehaviour
{
    public float TimeRemaining;

    private float totalTime;
    private new LineRenderer renderer;
    private Color c = new Color();

    public void Set(Vector3 start, Vector3 end)
    {
        renderer = GetComponent<LineRenderer>();
        renderer.SetPositions(new Vector3[] { start, end });
        totalTime = TimeRemaining;
    }

    public void Update()
    {
        TimeRemaining -= Time.deltaTime;

        float p = TimeRemaining / totalTime;

        c = renderer.startColor;
        c.a = p;
        renderer.startColor = c;
        c = renderer.endColor;
        c.a = p / 5f;
        renderer.endColor = c;

        if (TimeRemaining <= 0)
        {
            Destroy(this.gameObject);
        }
    }

}
