using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class KillFeedEnter : MonoBehaviour
{
    [Range(0f, 1f)]
    public float Percentage = 1f;
    public AnimationCurve Curve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    public Vector2 XAxis = new Vector2(0, 0);

    public void Update()
    {
        XAxis.x = -XAxis.y + GetComponent<KillFeedPanel>().GetWidth();
        float start = XAxis.x;
        float end = XAxis.y;
        float p = Curve.Evaluate(Percentage);
        float y = (transform as RectTransform).anchoredPosition.y;
        (transform as RectTransform).anchoredPosition = new Vector2(Mathf.Lerp(start, end, p), y);

    }

}
