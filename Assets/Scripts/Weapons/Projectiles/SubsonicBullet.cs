
using UnityEngine;
using UnityEngine.Networking;

public class SubsonicBullet : MonoBehaviour
{
    [Header("Controls")]
    public float Speed = 10f;
    public Vector2 Damage;
    public float MaxRange = 30f;

    [Header("Falloff")]
    public AnimationCurve Curve;

    [Header("Shooter")]
    public string FriendlyTeam;
    public string Shooter;
    public string Weapon;

    [Header("References")]
    public Transform Tip;
    public SpriteRenderer Light;
    public SpriteRenderer Bullet;

    [Header("Visuals")]
    public float InitialLightAlpha = 0.1f;
    public AnimationCurve AlphaDropoff = AnimationCurve.Linear(0, 0, 1, 1);

    private Vector2 startPos;

    public void Update()
    {
        Vector2 a = transform.position;
        Vector2 movement = transform.right * Time.deltaTime * Speed;
        Vector2 b = a + movement;

        UpdateAlpha();

        RaycastHit2D hit;
        bool collides = DetectCollision(Tip.transform.position, (Vector2)Tip.transform.position + movement, out hit);

        if (collides)
        {
            // Spawn hit effect.
            float angle = Mathf.Atan2(hit.normal.y, hit.normal.x) * Mathf.Rad2Deg + 180;
            HitEffect.Spawn(hit.point, angle, 15, Color.yellow, HitEffectPreset.Sparks);

            // Deal damage to the object if we are on the server.
            HitObject(hit);

            // Pool
            Recycle();

            // Exit early.
            return;
        }
        else
        {
            transform.position = b;
        }

        CheckRange();
    }

    private float GetDamage(Vector2 hitPoint)
    {
        float d = Damage.x;
        float m = Damage.y;
        float dst = Vector2.Distance(startPos, hitPoint);
        if (dst > MaxRange)
        {
            // Should never happen, but might be possible because of small mathematical inaccuracies.
            return 0;
        }
        float p = dst / MaxRange;
        float f = Curve.Evaluate(p);
        float x = Mathf.LerpUnclamped(m, d, f);


        float final = x; // Apply damage falloff.

        return final;
    }

    private void UpdateAlpha()
    {
        float a = Mathf.Clamp(DistanceFromSpawn() / MaxRange, 0f, 1f);
        a = AlphaDropoff.Evaluate(a);

        Color l = Light.color;
        l.a = Mathf.Lerp(a, 0f, InitialLightAlpha);
        Light.color = l;

        //Color b = Bullet.color;
        //b.a = a;
        //Bullet.color = b;
    }

    private float DistanceFromSpawn()
    {
        float distance = Vector2.Distance(Tip.transform.position, startPos);
        return distance;
    }

    private void CheckRange()
    {
        float distance = DistanceFromSpawn();

        if(distance > MaxRange)
        {
            Recycle();
        }
    }

    private void HitObject(RaycastHit2D hit)
    {
        if(Player.Local != null)
        {
            if (!Player.Local.isServer)
                return;
        }
        else
        {
            return;
        }

        if(Health.CanDamageObject(hit.collider, FriendlyTeam))
        {
            hit.collider.GetComponentInParent<Health>().ServerDamage(GetDamage(hit.point), Shooter + ":" + Weapon, false);
        }
    }

    private bool DetectCollision(Vector2 a, Vector2 b, out RaycastHit2D collision)
    {
        RaycastHit2D[] hits = Physics2D.LinecastAll(a, b);

        foreach(var hit in hits)
        {
            bool shouldHit = Health.CanHitObject(hit.collider, FriendlyTeam);

            if (shouldHit)
            {
                collision = hit;
                return true;
            }
        }

        collision = new RaycastHit2D();
        return false;
    }

    public void Init(Vector2 pos, Quaternion rotation, float speed, string team, string shooter, string weapon, Vector2 damage, AnimationCurve curve, float range)
    {
        transform.position = pos;
        transform.rotation = rotation;
        Speed = speed;
        FriendlyTeam = team;
        Shooter = shooter;
        Weapon = weapon;
        Damage = damage;
        Curve = curve;
        MaxRange = range;

        startPos = pos;

        UpdateAlpha();
    }

    private void Recycle()
    {
        ObjectPool.Destroy(this.gameObject, PoolType.SUBSONIC_BULLET);
    }

    public static void Spawn(Vector2 pos, float angle, float speed, string team, string shooter, string weapon, Vector2 damage, AnimationCurve curve, float range)
    {
        SubsonicBullet sb = ObjectPool.Instantiate(Spawnables.I.SubsonicBullet, PoolType.SUBSONIC_BULLET).GetComponent<SubsonicBullet>();

        Quaternion rotation = Quaternion.Euler(0, 0, angle);

        sb.Init(pos, rotation, speed, team, shooter, weapon, damage, curve, range);
    }

    public static void Spawn(Vector2 pos, Quaternion rotation, float speed, string team, string shooter, string weapon, Vector2 damage, AnimationCurve curve, float range)
    {
        SubsonicBullet sb = ObjectPool.Instantiate(Spawnables.I.SubsonicBullet, PoolType.SUBSONIC_BULLET).GetComponent<SubsonicBullet>();
        sb.Init(pos, rotation, speed, team, shooter, weapon, damage, curve, range);
    }
}