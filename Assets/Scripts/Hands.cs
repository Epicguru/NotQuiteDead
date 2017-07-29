using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hands : MonoBehaviour {

    public Weapon[] Weapons = new Weapon[3];
    public Transform Rotation;
    [Range(0, 3)]
    public int Holding;

    public bool Running;
    public float RunSpeed;
    public bool Aiming;
    public bool Right;

    public LayerMask ShootableLayers;

    private const string RUNNING = "Running";
    private const string AIMING = "Aiming";
    private const string RUN_SPEED = "RunSpeed";
    private const string SHOOT = "Shoot";
    private const string SHOOT_SPEED = "ShootSpeed";
    private const string STORED = "Stored";

    private float p;
    private Vector3 pos = new Vector3();

    // Update is called once per frame
    void Update () {

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Holding = 0;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Holding = 1;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Holding = 2;
        }

        Weapon weapon = Weapons[this.Holding];

        // Deactivate other weapons
        this.DeactivateStoredWeapons();

        if (weapon == null)
            return;

        weapon.gameObject.SetActive(true);

        weapon.transform.SetParent(Rotation);
        weapon.Stored = false;

        if (!Aiming)
        {
            pos.Set(0, 0, 0);
            weapon.gameObject.transform.localPosition = pos;
            Rotation.localRotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            Vector3 v3 = Input.mousePosition;
            v3.z = 0;
            v3 = Camera.main.ScreenToWorldPoint(v3);

            float dstX = Rotation.position.x - v3.x;
            float dstY = v3.y - Rotation.position.y;

            if (Right)
            {
                dstY = Rotation.position.y - v3.y;
            }

            float angle = Mathf.Atan2(dstY, dstX) * Mathf.Rad2Deg;

            Rotation.localRotation = Quaternion.Euler(0, 0, Right ? angle + 180 : angle);
        }

        Animator a = weapon.GetComponentInChildren<Animator>();

        a.SetBool(STORED, false);
        a.SetBool(RUNNING, Running);
        a.SetBool(AIMING, Aiming);
        a.SetFloat(RUN_SPEED, RunSpeed);

        if (Aiming && Input.GetMouseButton(0) && weapon.BulletInChamber)
        {
            a.SetBool(SHOOT, true);
        }
        else
        {
            a.SetBool(SHOOT, false);
        }
    }

    public void DeactivateStoredWeapons()
    {
        foreach(Weapon w in this.Weapons)
        {
            if (w == null)
                continue;

            Weapon stored = w;

            if (w == this.Weapons[this.Holding])
                continue;

            stored.gameObject.SetActive(true);

            if (stored != null)
            {
                Animator storedAnim = stored.GetComponentInChildren<Animator>();
                storedAnim.SetBool(STORED, true);
                storedAnim.SetBool(AIMING, false);
                storedAnim.SetBool(SHOOT, false);
                storedAnim.SetBool(RUNNING, false);
                stored.transform.SetParent(this.transform);
                stored.Stored = true;
            }
        }
    }

    public void OnGUI()
    {
        Weapon weapon = Weapons[this.Holding];
        if(weapon == null)
        {
            GUI.Label(new Rect(10, 10, 100, 100), "Null Weapon");
        }
        else
        {
            GUI.Label(new Rect(10, 10, 500, 100), weapon.gameObject.name);
            GUI.Label(new Rect(10, 30, 500, 100), weapon.BulletInChamber ? "Bullet In Chamber" : "No Bullet In Chamber");
            GUI.Label(new Rect(10, 60, 500, 100), weapon.Shooting.BulletsInMag + " bullets in mag (" + (weapon.Shooting.BulletsInMag + (weapon.BulletInChamber ? 1 : 0)) +  ")");
        }
    }
}