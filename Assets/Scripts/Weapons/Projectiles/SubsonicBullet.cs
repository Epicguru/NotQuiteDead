
using UnityEngine;
using UnityEngine.Networking;

public class SubsonicBullet : MonoBehaviour
{
    [Header("Controls")]
    public float Speed = 10f;
    public float Damage = 20f;

    [Header("Shooter")]
    public string FriendlyTeam;
    public string Shooter;
    public string Weapon;

    [Header("References")]
    public Transform Tip;

    public void Update()
    {
        Vector2 a = transform.position;
        Vector2 movement = transform.right * Time.deltaTime * Speed;
        Vector2 b = a + movement;

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
        }
        else
        {
            transform.position = b;
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
            hit.collider.GetComponentInParent<Health>().ServerDamage(Damage, Shooter + ":" + Weapon, false);
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

    public void Init(Vector2 pos, Quaternion rotation, float speed, string team, string shooter, string weapon, float damage)
    {
        transform.position = pos;
        transform.rotation = rotation;
        Speed = speed;
        FriendlyTeam = team;
        Shooter = shooter;
        Weapon = weapon;
        Damage = damage;
    }

    private void Recycle()
    {
        ObjectPool.Destroy(this.gameObject, PoolType.SUBSONIC_BULLET);
    }

    public static void Spawn(Vector2 pos, float angle, float speed, string team, string shooter, string weapon, float damage)
    {
        SubsonicBullet sb = ObjectPool.Instantiate(Spawnables.I.SubsonicBullet, PoolType.SUBSONIC_BULLET).GetComponent<SubsonicBullet>();

        Quaternion rotation = Quaternion.Euler(0, 0, angle);

        sb.Init(pos, rotation, speed, team, shooter, weapon, damage);
    }

    public static void Spawn(Vector2 pos, Quaternion rotation, float speed, string team, string shooter, string weapon, float damage)
    {
        SubsonicBullet sb = ObjectPool.Instantiate(Spawnables.I.SubsonicBullet, PoolType.SUBSONIC_BULLET).GetComponent<SubsonicBullet>();
        sb.Init(pos, rotation, speed, team, shooter, weapon, damage);
    }
}