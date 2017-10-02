using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetDirectionProvider :DirectionProvider {

    public Transform Target;
    public float FalloffStart = 5, FalloffEnd = 2;
    [Range(0f, 1f)]
    public float BaseWeight = 1f;
    public AnimationCurve Curve = AnimationCurve.Linear(1, 1, 0, 0);

	public override void Update ()
    {
        Active = Target != null;

        float p = 1f;

        if (FalloffEnd > FalloffStart)
            FalloffEnd = FalloffStart;

        if(Target != null)
        {
            float dst = Vector2.Distance(Target.position, transform.position);
            if(dst <= FalloffStart)
            {
                float x = (dst - FalloffEnd) / (FalloffStart - FalloffEnd);
                p = Mathf.Clamp(Curve.Evaluate(x), 0f, 1f);
            }
        }

        if (Target != null)
            Direction = Target.position - transform.position;
        else
            Direction = Vector2.zero;
        Weight = BaseWeight * p;

        base.Update();
	}
}
