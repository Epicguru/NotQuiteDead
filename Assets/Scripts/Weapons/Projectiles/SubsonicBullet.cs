
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

    public void Update()
    {
        Vector2 a = transform.position;
        Vector2 b = a + (Vector2)(transform.right * Time.deltaTime * Speed);

        RaycastHit2D hit;
        bool collides = DetectCollision(a, b, out hit);

        if (collides)
        {
            transform.position = hit.point;
            HitObject(hit);
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

        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        sb.Init(pos, rotation, speed, team, shooter, weapon, damage);
    }

    public static void Spawn(Vector2 pos, Quaternion rotation, float speed, string team, string shooter, string weapon, float damage)
    {
        SubsonicBullet sb = ObjectPool.Instantiate(Spawnables.I.SubsonicBullet, PoolType.SUBSONIC_BULLET).GetComponent<SubsonicBullet>();
        sb.Init(pos, rotation, speed, team, shooter, weapon, damage);
    }
}