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
    [SyncVar]
    public bool IsDropped;

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
        if (IsDropped)
        {
            Animation.Animator.SetBool(Animation.Dropped, true);
            Hand.SetEquipHandRender(transform, false);
        }
        if (!Item.IsEquipped() && isServer)
        {
            // Assume dropped...
            if (!IsDropped)
                IsDropped = true;
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
        int random = 0;
        if (randomize)
            random = Randomize(Animation.RequireNewRandom);

        // This takes place on the local client...
        Animation.Animator.SetInteger(Animation.Random, random);
        Animation.Animator.SetTrigger(Animation.Attack);

        // Tell other clients what we are doing...
        // On their end, animations will play but the authoritive version of events (animation speed, cancelling etc.)
        // will happen here on the client. May be a bad idea. Oh well. This is because if the local client plays the animation
        // when the RPC is recieved, then attack speed will vary depending on ping. Not gud.
        // On the other hand, this will mean that if there is a high ping then attacks will seem irregular and desynchronised
        // for other clients. Bloody multiplayer :)
        CmdRandomizeAnims(random);
        CmdAnimAttack();

        // Flag as attacking
        IsAttacking = true;
    }

    public int Randomize(bool requireNew)
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

        return random;
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

        int random = 0;
        if (randomize)
            random = Randomize(Animation.RequireNewRandom);

        Animation.Animator.SetInteger(Animation.Random, random);
        Animation.Animator.SetTrigger(Animation.Equip);

        CmdRandomizeAnims(random);
        CmdAnimEquip();
        IsEquiping = true;
    }

    [Command]
    private void CmdRandomizeAnims(int random)
    {
        RpcRandomizeAnims(random);
    }

    [ClientRpc]
    private void RpcRandomizeAnims(int value)
    {
        if(Item != Player.Local.Holding.Item)
            Animation.Animator.SetInteger(Animation.Random, value);
    }

    [Command]
    private void CmdAnimEquip()
    {
        RpcAnimEquip();
    }

    [ClientRpc]
    private void RpcAnimEquip()
    {
        if (Item != Player.Local.Holding.Item)
            Animation.Animator.SetTrigger(Animation.Equip);
    }

    [Command]
    private void CmdAnimAttack()
    {
        RpcAnimAttack();
    }

    [ClientRpc]
    private void RpcAnimAttack()
    {
        if (Item != Player.Local.Holding.Item)
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