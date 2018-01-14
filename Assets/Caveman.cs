using UnityEngine;
using UnityEngine.Networking;

public class Caveman : NetworkBehaviour
{
    public Pawn Pawn;

    public void Start()
    {
        Pawn.Health.UponDeathServer += UponDeath;
    }

    public void Update()
    {
        if (!isServer)
            return;

        if (Player.Local == null)
            return;

        Pawn.Path.SetTarget((int)Player.Local.transform.position.x, (int)Player.Local.transform.position.y);
    }

    [Server]
    public void UponDeath()
    {
        Debug.Log("Dead!");

        Destroy(gameObject);
    }
}