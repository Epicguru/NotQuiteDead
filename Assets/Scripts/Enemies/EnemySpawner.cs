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
        float secondsPerDay = GameTime.Instance.SecondsPerDay;
        foreach (var info in Enemies)
        {
            // Chance per frame:
            // CpD  * Time.deltaTime / SecondsPerDay

            int hourRange = info.MaxHour - info.MinHour;
            float partOfDay = hourRange / 24f;

            float enemiesPerDay = info.GetSpawnRate(day);
            float enemiesPerPart = enemiesPerDay / (partOfDay);

            float enemiesPerSecond = enemiesPerPart / secondsPerDay;
            float enemiesPerFrame = enemiesPerSecond * Time.deltaTime;

            bool spawn = Random.value <= enemiesPerFrame;

            if (spawn)
            {
                Spawn(info.Pawn.Prefab, info.SpawnType);
            }
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