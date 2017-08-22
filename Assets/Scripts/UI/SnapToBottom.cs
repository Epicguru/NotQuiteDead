using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class SnapToBottom : MonoBehaviour
{
    public RectTransform Target;
    private static Vector2 temp = new Vector2();

    // Moves this rect to the bottom of the other rect, assuming that the pivot is the top.

    public void Update()
    {
        temp.Set(Target.localPosition.x, Target.localPosition.y);

        temp.y -= Target.sizeDelta.y;

        (transform as RectTransform).localPosition = temp;
    }
}
