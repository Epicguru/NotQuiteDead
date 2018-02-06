using System;
using UnityEngine;

[Serializable]
public class EnemySpawningInfo
{
    [Header("Enemy")]
    public Pawn Pawn;

    [Header("Spawn Period")]
    public int MinDay = 0;
    public int MaxDay = -1;

    [Header("Spawn Time")]
    public int MinHour = 21;
    public int MaxHour = 24;

    [Header("Spawn Rate")]
    [Tooltip("The amount of enemies of this type that will spawn in one day-night cycle (not guaranteed, average). Controlled by the spawn rate curve. If MaxDay < 0, then the curve has no effect.")]
    public float BaseSpawnRate = 10f;
    public AnimationCurve SpawnRateCurve = AnimationCurve.Linear(0, 1, 1, 1);

    [Header("Spawn Type")]
    public EnemySpawnType SpawnType = EnemySpawnType.AROUND_RANDOM_PLAYER;

    public float GetSpawnRate(int day)
    {
        if (day < 0)
            return 0f;

        if(MaxDay < 0)
        {
            if (day < MinDay)
                return 0;
            else
                return BaseSpawnRate;
        }
        else
        {
            if (day < MinDay)
                return 0;

            int d = day - MinDay;
            int range = MaxDay - MinDay;
            float p = (float)d / (float)range;

            float x = SpawnRateCurve.Evaluate(p);

            float rate = BaseSpawnRate * x;

            return rate;
        }
    }
}