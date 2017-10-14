using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimationSync : MonoBehaviour
{
    public bool Active = true;
    public float AttackRange = 2f;
    public float DirectionOverrideRange = 2f;
    public float IdleDeadzone = 0.1f;

    [HideInInspector] public NetPositionSync NetPos;
    [HideInInspector] public TargetDirectionProvider Targeting;
    [HideInInspector] public EnemyDirection Direction;
    [HideInInspector] public EnemyAnimation Animation;

    public void Start()
    {
        NetPos = GetComponent<NetPositionSync>();
        Targeting = GetComponent<TargetDirectionProvider>();
        Direction = GetComponent<EnemyDirection>();
        Animation = GetComponent<EnemyAnimation>();
    }

    public void Update()
    {
        if (!Active)
            return;

        float xVel = NetPos.Velocity.x;
        float yVel = NetPos.Velocity.y;
        bool walking = true;

        if(xVel <= IdleDeadzone && xVel >= -IdleDeadzone)
        {
            if(yVel <= IdleDeadzone && yVel >= -IdleDeadzone)
                walking = false; // Stop walking!
        }
        else if(xVel > IdleDeadzone)
        {
            Direction.Right = true;
        }
        else if(xVel < -IdleDeadzone)
        {
            Direction.Right = false;
        }

        bool hasTarget = Targeting.Target != null;
        float dst = hasTarget ? Vector2.Distance(Targeting.Target.position, transform.position) : 0f;
        bool overrideDirection = hasTarget && dst <= DirectionOverrideRange;

        if (overrideDirection)
        {
            bool newRight = transform.position.x < Targeting.Target.position.x;

            Direction.Right = newRight;
        }

        bool inAttackRange = hasTarget ? dst <= AttackRange : false;

        Animation.Attacking = inAttackRange;
        Animation.Walking = walking;
    }
}
