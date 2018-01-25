
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Pawn), typeof(PawnAnimation))]
public class PawnMeleeAttack : NetworkBehaviour
{
    [HideInInspector]
    public Pawn Pawn;

    [HideInInspector]
    public PawnAnimation Animation;

    [Header("Target")]
    [ReadOnly]
    public Health Target;
    public PawnMeleeTarget TargetType = PawnMeleeTarget.PLAYER;

    [Header("Attacking")]
    public float AttackRange = 3f;
    public float AttackDamage = 20f;

    public void Awake()
    {
        Animation = GetComponent<PawnAnimation>();
        Pawn = GetComponent<Pawn>();
    }

    public void Update()
    {
        if (!isServer)
        {
            return;
        }

        // Ensure that if we have a target it is still a valid target.
        VerifyTarget();

        // If we do not have a target, then find a new one.
        SelectTarget();

        // Cause the attack animation to player.
        UpdateAnimation();
    }

    [Server]
    private void UpdateAnimation()
    {
        // Cause the animation to play if in attacking range.
        if (Target == null)
            return;

        float dst = DistanceToTarget();
        if(dst <= AttackRange)
        {
            Animation.Attacking = true;
        }
        else
        {
            Animation.Attacking = false;
        }
    }

    [ServerCallback]
    public virtual void HitTarget()
    {
        VerifyTarget();
        if(Target != null)
            Target.ServerDamage(AttackDamage, Pawn.Name, false);
    }

    [Server]
    public virtual void VerifyTarget()
    {
        if (Target == null)
            return;

        if (Target.GetHealth() <= 0)
            Target = null;
        if (DistanceToTarget() > AttackRange)
            Target = null;
    }

    public float DistanceToTarget()
    {
        if (Target == null)
            return -1f;

        return Vector2.Distance(transform.position, Target.transform.position);
    }

    [Server]
    public virtual void SelectTarget()
    {
        if (Target != null)
            return;

        switch (TargetType)
        {
            case PawnMeleeTarget.PLAYER:

                // Find the closest player...

                float minDst = float.MaxValue;
                Player closest = null;
                foreach(Player p in Player.AllPlayers)
                {
                    float dst = Vector2.Distance(transform.position, p.transform.position);
                    if(dst < minDst)
                    {
                        closest = p;
                        minDst = dst;
                    }
                }

                if(closest != null)
                {
                    Target = closest.Health;
                }

                break;
        }
    }
}

public enum PawnMeleeTarget
{
    PLAYER
}