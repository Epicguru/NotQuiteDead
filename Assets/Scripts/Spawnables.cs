using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawnables : MonoBehaviour {

    public static Spawnables I;

    public VisualBullet BulletTrail;
    public Enemy Zombie;
    public GameObject BloodSpurt;
    public GameObject BloodBleeding;
    public Sprite[] BloodParticles;
    public Sprite[] BloodPools;
    public GameObject WeaponContainer;
    public GameObject GunSparks;
    public GameObject GunSmoke;

    public void Start()
    {
        I = this;
    }
}