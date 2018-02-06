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
    public int day = 0;
    public int spawned = 0;

    public void Update()
    {
        if (!isServer)
            return;

        int day = GameTime.Instance.GetDay();
        int hour = GameTime.Instance.GetHour();
        float secondsPerDay = GameTime.Instance.SecondsPerDay;
        foreach (var info in Enemies)
        {
            if (hour < info.MinHour)
                return;
            if (hour > info.MaxHour)
                return;

            info.Timer += Time.deltaTime;

            float partOfDay = (info.MaxHour - info.MinHour) / 24f;
            float perDay = info.GetSpawnRate(day);

            float daySection = (1f / perDay) * partOfDay;

            float seconds = (daySection * secondsPerDay) / SpawnMultiplier;

            while(info.Timer >= seconds)
            {
                info.Timer -= seconds;

                Spawn(info.Pawn.Prefab, info.SpawnType);
                spawned++;
            }
        }

        if(day != this.day)
        {
            Debug.Log("In this day spawned " + spawned + " enemies.");
            spawned = 0;
            this.day = day;
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