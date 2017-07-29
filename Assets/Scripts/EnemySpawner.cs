using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour {

    public float SpawnRate = 0.1f; // Per Second
    public Transform Target;
    public float MinDistance, MaxDistance;
    public GameObject[] Enemies;

    private float timer;

    public void Update()
    {
        timer += Time.deltaTime;
        while(timer >= SpawnRate)
        {
            timer -= SpawnRate;

            SpawnEnemy();
        }
    }

    public void SpawnEnemy()
    {
        float distance = Random.Range(MinDistance, MaxDistance);
        float angle = Random.Range(0, 360);
        GameObject enemy = Enemies[Random.Range(0, Enemies.Length - 1)];

        float x = Target.position.x + Mathf.Cos(angle * Mathf.Deg2Rad) * distance;
        float y = Target.position.y + Mathf.Sin(angle * Mathf.Deg2Rad) * distance;

        GameObject g = Instantiate(enemy, new Vector3(x, y, 0), Quaternion.identity);

        Enemy e = g.GetComponentInChildren<Enemy>();
        e.Target = GameObject.Find("Player").transform;
    }
}