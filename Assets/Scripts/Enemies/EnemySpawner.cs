using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class EnemySpawner : NetworkBehaviour
{
    [SerializeField]
    public List<EnemySpawningInfo> Enemies = new List<EnemySpawningInfo>();

    public bool SpawnEnemies = true;
    public float SpawnMultiplier = 1f;

    public void Update()
    {
        if (!isServer)
            return;

        int day = GameTime.Instance.GetDay();
        foreach (var info in Enemies)
        {
            // Chance per frame:
            // CpD  * Time.deltaTime / SecondsPerDay

            float enemiesPerDay = info.GetSpawnRate(day);
        }
    }

    [Server]
    public void Spawn(string pawn, EnemySpawnType type)
    {
        switch (type)
        {
            case EnemySpawnType.AROUND_RANDOM_PLAYER:

                float minDst = 15f;
                float maxDst = 25f;
                float dst = Random.Range(minDst, maxDst);
                Vector2 point = Random.insideUnitCircle.normalized;
                point *= dst;

                Vector2 position = (Vector2)Player.AllPlayers[Random.Range(0, Player.AllPlayers.Count)].transform.position + point;

                Pawn.SpawnPawn(pawn, position);

                break;
        }
    }
}