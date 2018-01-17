
using UnityEngine;
using UnityEngine.Networking;

public class TurretTargeting : NetworkBehaviour
{
    [Header("System")]
    public Transform center;
    public TurretAiming Aim;
    public Turret Turret;
    public TurretShooting Shooting;

    [Header("Targeting")]
    public bool Active;
    [Tooltip("Does this shoot through other non-solid entities in an attempt to hit the target?")]
    public bool FireInTheHole = true;
    public Pawn Target;

    [Header("Lerping")]
    [ReadOnly]
    public float LerpRate = 5f;
    public Vector2 ActiveAndIdleLerpRates = new Vector2(5f, 1.5f);

    [Header("Wandering")]
    public Vector2 WanderInterval = new Vector2(0.5f, 2f);
    public Vector2 WanderAmount = new Vector2(-50f, 50f);

    [Header("Data")]
    [ReadOnly]
    public bool ShouldShoot;
    [ReadOnly]
    public float TargetAngle;
    [ReadOnly]
    public float MaxSearchRange;

    private float timer;
    private float interval = 0.5f;

    public void Update()
    {
        if (isServer)
        {
            if (!Active)
            {
                // Server, not enabled.
                Target = null;
                TargetAngle = 0;
                ShouldShoot = false;
                LerpRate = ActiveAndIdleLerpRates.y;
            }
            else
            {
                FindTarget();
                if(Target != null)
                {
                    LerpRate = ActiveAndIdleLerpRates.x;
                    TargetAngle = GetAngleToTarget();

                    // Test to shoot:
                    // 1. Is the target beyond the minimum range?
                    // 2. Can I hit the target? As in: Is there an impenetrable object before I hit the target?

                    float dst = Vector2.Distance(center.transform.position, Target.transform.position);
                    if(dst <= Turret.Deadzone)
                    {
                        // Inside the deadzone.
                        ShouldShoot = false;
                    }
                    else
                    {
                        // Raycast test.
                        RaycastHit2D[] hits = Physics2D.RaycastAll(Shooting.Muzzle.transform.position, Shooting.Muzzle.transform.up, Turret.Range);
                        bool clearPath = true;
                        foreach(var hit in hits)
                        {
                            // Something solid, that we can hit...
                            if(Health.CanHitObject(hit.collider, null)) // Allow this algorithm take into account friendly players to not shoot when they stand in front.
                            {
                                // Should be damaged, cannot be shot though!
                                if(!Health.CanDamageObject(hit.collider, Shooting.Team))
                                {
                                    clearPath = false;
                                    break;
                                }
                                else
                                {
                                    // Is it the target?
                                    if (hit.transform.GetComponentInParent<Pawn>() == Target)
                                    {
                                        clearPath = true;
                                        break;
                                    }
                                    else
                                    {
                                        // There is an object that can take damage standing between us and the target...
                                        // Should we shoot though it?
                                        if (FireInTheHole)
                                        {
                                            continue;
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }
                                }
                            }
                        }

                        // Clearpath means that we COULD hit the target.
                        ShouldShoot = clearPath;
                    }
                }
                else
                {
                    LerpRate = ActiveAndIdleLerpRates.y;
                    ShouldShoot = false;
                    UpdateWandering();
                }

            }

            LerpToAngle();
        }
    }

    [Server]
    private void UpdateWandering()
    {
        timer += Time.deltaTime;

        if(timer > interval)
        {
            timer = 0;
            interval = Random.Range(WanderInterval.x, WanderInterval.y);
            TargetAngle += Random.Range(WanderAmount.x, WanderAmount.y);
        }
    }

    [Server]
    private void FindTarget()
    {
        if(Target != null)
        {
            float dst = Vector2.Distance(center.position, Target.transform.position);
            if(dst > MaxSearchRange)
            {
                Target = null;
            }
            else
            {
                if (Target.Health.GetHealth() <= 0)
                {
                    Target = null;
                }
            }
        }

        foreach (Pawn p in Pawn.EnemyPawns)
        {
            if(p.Health.GetHealth() > 0)
            {
                float dst = Vector2.Distance(p.transform.position, center.position);
                if(dst <= MaxSearchRange)
                {
                    Target = p;
                    return;
                }
            }
        }
    }

    private float GetAngleToTarget()
    {
        if (Target == null)
            return 0;

        float deltaY = Target.transform.position.y - center.position.y;
        float deltaX = Target.transform.position.x - center.position.x;

        return Mathf.Rad2Deg * Mathf.Atan2(deltaY, deltaX) - 90;
    }

    [Server]
    private void LerpToAngle()
    {
        float a = Mathf.LerpAngle(Aim.Angle, TargetAngle, Time.deltaTime * LerpRate);
        if (Aim.Angle != a)
            Aim.Angle = a;
    }
}