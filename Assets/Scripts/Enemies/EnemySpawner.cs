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
            if (day < info.MinDay)
                return;
            if (day > info.MaxDay)
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
            Debug.Log("In this previous day, " + spawned + " enemies spawned.");
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

                // Find a player to spawn at, must not be dead.
                Player player;
                do
                {
                    player = Player.AllPlayers[Random.Range(0, Player.AllPlayers.Count)];
                }
                while (player == null || player.Health.IsDead);

                // Set min and max spawn distances
                float minDst = 20f;
                float maxDst = 30f;

                // Get the player's location.
                Vector2 playerPos = ((Vector2)player.transform.position.ToInt()).ClampToWorldIndex(World.Instance);

                // Make an offset until it is a valid position...
                Vector2 offset;
                Vector2 position;
                int loop = 0;
                do
                {
                    offset = Random.insideUnitCircle.normalized;
                    offset *= Random.Range(minDst, maxDst);
                    offset = offset.ToInt();

                    position = (playerPos + offset).ClampToWorldIndex(World.Instance);

                    // Avoid infinite loop when the enemy simply can't be spawned...
                    loop++;
                    if(loop == 100)
                    {
                        Debug.LogError("Enemy {0} can't be spawned after trying to find a spot {1} times! Centered around player {2}. Spawning right on top of player :D".Form(pawn, loop, player.Name));
                        position = playerPos + new Vector2(0.5f, 0.5f);
                        break;
                    }
                }
                while (!World.Instance.TileMap.GetLayer("Foreground").IsSpotWalkable((int)position.x, (int)position.y));

                Pawn.SpawnPawn(pawn, position + new Vector2(0.5f, 0.5f));

                break;
        }
    }
}