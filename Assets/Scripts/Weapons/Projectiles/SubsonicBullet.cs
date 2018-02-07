
using UnityEngine;
using UnityEngine.Networking;

public class SubsonicBullet : NetworkBehaviour
{
    [Header("Controls")]
    public float Speed = 10f;

    [Header("Shooter")]
    public string FriendlyTeam;
    public string Shooter;
    public string Weapon;

    public void Update()
    {
        if (!isServer)
        {
            return;
        }

        Vector2 a = transform.position;
        Vector2 b = a + (Vector2)(transform.right * Time.deltaTime * Speed);

        bool collides = DetectCollision(a, b);

        if (collides)
        {
            Recycle();
        }
        else
        {
            transform.position = b;
        }
    }

    private bool DetectCollision(Vector2 a, Vector2 b)
    {
        RaycastHit2D[] hits = Physics2D.LinecastAll(a, b);

        foreach(var hit in hits)
        {
            bool shouldHit = Health.CanHitObject(hit.collider, FriendlyTeam);

            if (shouldHit)
                return true;
        }

        return false;
    }

    public void Init(Vector2 pos, Quaternion rotation, float speed, string team, string shooter, string weapon)
    {
        transform.position = pos;
        transform.rotation = rotation;
        Speed = speed;
        FriendlyTeam = team;
        Shooter = shooter;
        Weapon = weapon;
    }

    private void Recycle()
    {
        ObjectPool.Destroy(this.gameObject, PoolType.SUBSONIC_BULLET);
    }

    public static void Spawn(Vector2 pos, float angle, float speed, string team, string shooter, string weapon)
    {
        SubsonicBullet sb = ObjectPool.Instantiate(Spawnables.I.SubsonicBullet, PoolType.SUBSONIC_BULLET).GetComponent<SubsonicBullet>();

        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        sb.Init(pos, rotation, speed, team, shooter, weapon);
    }

    public static void Spawn(Vector2 pos, Quaternion rotation, float speed, string team, string shooter, string weapon)
    {
        SubsonicBullet sb = ObjectPool.Instantiate(Spawnables.I.SubsonicBullet, PoolType.SUBSONIC_BULLET).GetComponent<SubsonicBullet>();
        sb.Init(pos, rotation, speed, team, shooter, weapon);
    }
}