using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

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

    public void AnimAttack(bool randomize = true)
    {
        if (!hasAuthority)
        {
            Debug.LogError("No authority to call animation with this weapon!");
            return;
        }
        if (IsAttacking)
        {
            Debug.LogError("Already attacking!");
            return;
        }
        CmdAnimAttack(randomize);
        IsAttacking = true;
    }

    public void AnimEquip(bool randomize = true)
    {
        if (!hasAuthority)
        {
            Debug.LogError("No authority to call animation with this weapon!");
            return;
        }
        if(IsEquiping)
        {
            Debug.LogError("Already equiping!");
            return;
        }

        CmdAnimEquip(randomize);
        IsEquiping = true;
    }

    [Command]
    private void CmdRandomizeAnims(bool requireNew)
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

        RpcRandomizeAnims(random);
    }

    [ClientRpc]
    private void RpcRandomizeAnims(int value)
    {
        Animation.Animator.SetInteger(Animation.Random, value);
    }

    [Command]
    private void CmdAnimEquip(bool randomize)
    {
        if (randomize)
            CmdRandomizeAnims(Animation.RequireNewRandom);
        else
            RpcRandomizeAnims(0);

        RpcAnimEquip();
    }

    [ClientRpc]
    private void RpcAnimEquip()
    {
        Animation.Animator.SetTrigger(Animation.Equip);
    }

    [Command]
    private void CmdAnimAttack(bool randomize)
    {
        if (randomize)
            CmdRandomizeAnims(Animation.RequireNewRandom);
        else
            RpcRandomizeAnims(0);
        RpcAnimAttack();
    }

    [ClientRpc]
    private void RpcAnimAttack()
    {
        Animation.Animator.SetTrigger(Animation.Attack);
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