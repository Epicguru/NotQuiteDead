using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunManager : MonoBehaviour {

    public Gun[] Weapons = new Gun[3];
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
    private const string DROPPED = "Dropped";

    private float p;

    // Update is called once per frame
    void LateUpdate () {

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (Input.GetKey(KeyCode.Space))
            {
                Drop(0);
            }
            else
            {
                Holding = 0;
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (Input.GetKey(KeyCode.Space))
            {
                Drop(1);
            }
            else
            {
                Holding = 1;
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (Input.GetKey(KeyCode.Space))
            {
                Drop(2);
            }
            else
            {
                Holding = 2;
            }
        }

        Gun weapon = Weapons[this.Holding];

        // Deactivate other weapons
        this.DeactivateStoredWeapons();

        if (weapon == null)
            return;

        weapon.gameObject.SetActive(true);

        weapon.transform.SetParent(Rotation);
        weapon.Stored = false;
        weapon.Dropped = false;

        if (!Aiming)
        {
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
        a.SetBool(DROPPED, false);

        if (Aiming && Input.GetMouseButton(0) && weapon.BulletInChamber)
        {
            a.SetBool(SHOOT, true);
        }
        else
        {
            a.SetBool(SHOOT, false);
        }
    }

    public void Drop(int weapon)
    {
        if (Weapons[weapon] == null)
            return;

        Gun w = Weapons[weapon];
        GameObject prefab = w.GetPrefab();

        if(prefab == null)
        {
            Debug.LogError("Prefab on weapon was null! (" + w.name + ")");
            return;
        }

        Destroy(w.gameObject);
        Weapons[weapon] = null;

        GameObject container = Instantiate(Spawnables.I.WeaponContainer, transform.position, Quaternion.identity);
        GameObject newObject = Instantiate(prefab, container.transform);

        newObject.GetComponent<Gun>().Dropped = true;
    }

    public void Equip(GameObject weapon, int slot, bool equip = true)
    {
        if (slot > 0 && Weapons[slot] != null)
            return;

        Gun w = weapon.GetComponentInChildren<Gun>();
        if (w == null)
            return;

        GameObject prefab = w.GetPrefab();

        if (prefab == null)
            return;

        if (slot < 0)
            slot = this.GetNextAvailableSlot();
        if (slot == -1)
        {
            slot = Holding;
            Drop(Holding);
        }


        GameObject newWeapon = Instantiate(prefab, transform);

        Gun w2 = newWeapon.GetComponent<Gun>();

        this.Weapons[slot] = w2;

        Destroy(weapon);
        if (equip) this.Holding = slot;
    }

    public int GetNextAvailableSlot()
    {
        for(int i = 0; i < this.Weapons.Length; i++)
        {
            if(Weapons[i] == null)
            {
                return i;
            }
        }
        return -1;
    }

    public void DeactivateStoredWeapons()
    {
        foreach(Gun w in this.Weapons)
        {
            if (w == null)
                continue;

            Gun stored = w;

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
                storedAnim.SetBool(DROPPED, false);
                stored.transform.SetParent(this.transform);
                stored.Stored = true;
            }
        }
    }

    public void OnGUI()
    {
        Gun weapon = Weapons[this.Holding];
        if(weapon == null)
        {
            GUI.Label(new Rect(10, 10, 100, 100), "Null Gun");
        }
        else
        {
            GUI.Label(new Rect(10, 10, 500, 100), weapon.gameObject.name);
            GUI.Label(new Rect(10, 30, 500, 100), weapon.BulletInChamber ? "Bullet In Chamber" : "No Bullet In Chamber");
            GUI.Label(new Rect(10, 60, 500, 100), weapon.Shooting.BulletsInMag + " bullets in mag (" + (weapon.Shooting.BulletsInMag + (weapon.BulletInChamber ? 1 : 0)) +  ")");
        }
    }
}