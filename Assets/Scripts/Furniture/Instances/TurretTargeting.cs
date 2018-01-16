
using UnityEngine;
using UnityEngine.Networking;

public class TurretTargeting : NetworkBehaviour
{
    [Header("System")]
    public Transform center;
    public TurretAiming Aim;

    [Header("Targeting")]
    public bool Active;
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
                    ShouldShoot = true;
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
        Aim.Angle = Mathf.LerpAngle(Aim.Angle, TargetAngle, Time.deltaTime * LerpRate);
    }
}