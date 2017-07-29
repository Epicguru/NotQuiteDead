using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawnables : MonoBehaviour {

    public static Spawnables I;

    public VisualBullet BulletTrail;
    public BloodParticle BloodParticle;
    public Enemy Zombie;
    public Sprite[] BloodParticles;
    public Sprite[] BloodPools;

    public void Start()
    {
        I = this;
    }
}