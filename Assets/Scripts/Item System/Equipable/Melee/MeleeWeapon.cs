using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : Weapon {

    // This represents an extension of an item that is a meele weapon.
    // This contains data such as damage and other info.

    [Tooltip("Information about the damage that this weapon inflicts.")]
    public MeleeDamage Damage;

    [Tooltip("Information about animations for this weapon.")]
    public MeleeAnimatorInfo Animation;

    [HideInInspector] public Item Item;
    public int ComboNumber;

    public bool IsEquiping;
    public bool IsAttacking;

    public void Start()
    {
        // Get references
        Item = GetComponent<Item>();

        // Set type.
        this.Type = WeaponType.MELEE;

        object x = GetComponentInChildren<MeleeAnimationCallbacks>();

        if(x == null)
        {
            Debug.LogError("No Animation Callback object located.");
            Debug.Break();
        }
    }

    public void Update()
    {
        if (!Item.IsEquipped())
        {
            return;
        }

        if (Player.Local.Holding.Item != this.Item)
        {
            return;
        }

        UpdateSwinging();
        if (Input.GetKeyDown(KeyCode.R))
        {
            AnimEquip();
        }
    }

    public void UpdateSwinging()
    {
        if (IsAttacking == false && IsEquiping == false)
        {
            if(InputManager.InputPressed("Shoot"))
            {
                // Attack
                AnimAttack(Animation.FirstAttackIsNotRandom ? ComboNumber > 0 : true);
                ComboNumber++;
            }
            else
            {
                ComboNumber = 0;
            }
        }
    }

    public void RandomizeAnims(bool requireNew = false)
    {
        int current = Animation.Animator.GetInteger(Animation.Random);
        int random = Random.Range(Animation.MinRandom, Animation.MaxRandom + 1);
        if (requireNew)
        {
            while (random == current)
            {
                random = Random.Range(Animation.MinRandom, Animation.MaxRandom + 1);
            }
        }

        Animation.Animator.SetInteger(Animation.Random, random);
    }

    public void AnimEquip(bool randomize = true)
    {
        if(IsEquiping)
        {
            Debug.LogError("Already equiping!");
            return;
        }

        if (randomize)
            RandomizeAnims(Animation.RequireNewRandom);
        else
            Animation.Animator.SetInteger(Animation.Random, 0);

        Animation.Animator.SetTrigger(Animation.Equip);
        IsEquiping = true;
    }

    public void AnimAttack(bool randomize = true)
    {
        if (IsAttacking)
        {
            Debug.LogError("Already attacking!");
            return;
        }

        if (randomize)
            RandomizeAnims(Animation.RequireNewRandom);
        else
            Animation.Animator.SetInteger(Animation.Random, 0);
        Animation.Animator.SetTrigger(Animation.Attack);
        IsAttacking = true;
    }

    public void Callback_AttackEnd()
    {
        IsAttacking = false;
    }

    public void Callback_EquipEnd()
    {
        IsEquiping = false;
    }
}