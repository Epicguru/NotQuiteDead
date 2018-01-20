using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(BowAiming))]
public class Bow : Weapon {

    public float Damage = 50f; // TODO
    public float DrawTime = 0.5f;
    public float ArrowSpeed = 20f;
    public float Range = 25f;
    public AnimationCurve DrawCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public GameObject RealArrow;

    [HideInInspector]
    public Item Item;
    [HideInInspector]
    public Animator Animator;
    public float SendRate = 20f;

    private bool Drawing;
    [SyncVar(hook = "OnBowDrawChange")]
    public float P;
    [HideInInspector]
    public bool Released; // TODO Add minimum charge time!!!
    [HideInInspector]
    public bool InFire;
    private float timer;

    public void Start()
    {
        Item = GetComponent<Item>();
        Animator = GetComponentInChildren<Animator>();
    }

    public void Update()
    {
        Animator.SetBool("Dropped", !Item.IsEquipped());
        Hand.RenderHands(transform, Item.IsEquipped());

        if (!Item.IsEquipped())
            return;

        UpdateDrawTime();
        
        Animator.SetBool("Run", InputManager.InputPressed("Sprint") && Player.Local.GetComponent<Rigidbody2D>().velocity != Vector2.zero);
    }

    public void UpdateDrawTime()
    {
        if (hasAuthority)
        {
            if (InputManager.InputPressed("Aim") && !InFire)
            {
                P = Mathf.Clamp(P + Time.deltaTime * (1f / DrawTime), 0f, 1f);
                Released = false;
            }
            else
            {
                if (P == 1f && !Released)
                {
                    DrawReleased(P);
                    P = 0f;
                    Released = true;
                }
                if (P == 0f)
                    Released = false;
                P = Mathf.Clamp(P - Time.deltaTime * (1f / DrawTime), 0f, 1f);
            }
        }

        if (!isServer && hasAuthority)
        {
            timer += Time.deltaTime;
            if(timer >= 1f / SendRate)
            {
                timer -= 1f / SendRate;
                CmdSendP(P);       
            }
        }

        float x = Mathf.Clamp(DrawCurve.Evaluate(P), 0f, 0.995f);

        if (P > 0f)
        {
            Drawing = true;
        }
        else
        {
            Drawing = false;
        }

        Animator.SetBool("Drawing", Drawing);
        if (Drawing)
        {
            Animator.Play("Draw", 0, x);
        }
        else
        {
            // Other animations will play...
        }
    }

    [Command]
    public void CmdSendP(float p)
    {
        this.P = p;
    }

    public void DrawReleased(float force)
    {
        InFire = true;
        Animator.SetTrigger("Fire");
        CmdFireTrigger(); //Shoot on other clients.
    }

    public void OnBowDrawChange(float newValue)
    {
        if (hasAuthority)
            return; // Has the real version, even if it is not the client.
        this.P = newValue;
    }

    [Command]
    public void CmdFireTrigger()
    {
        RpcFireTrigger();
    }

    [ClientRpc]
    public void RpcFireTrigger()
    {
        if(!hasAuthority)
            Animator.SetTrigger("Fire");
    }

    public void CallbackArrowFire()
    {
        // All clients fire arrows... But other clients fire from rpc.
        if (hasAuthority)
        {
            Arrow.FireArrow(RealArrow.transform.position, RealArrow.transform.rotation, (InputManager.GetMousePos() - (Vector2)transform.parent.transform.position), ArrowSpeed, Range, Damage, Player.Local.Name + ":" + Item.Prefab, Player.Local.Team);
            CmdArrowFire(RealArrow.transform.position, RealArrow.transform.rotation, (InputManager.GetMousePos() - (Vector2)transform.parent.transform.position), Player.Local.Team);
        }
    }

    [Command]
    public void CmdArrowFire(Vector2 spawn, Quaternion rotation, Vector2 direction, string team)
    {
        RpcArrowFire(spawn, rotation, direction, team);
    }

    [ClientRpc]
    public void RpcArrowFire(Vector2 spawn, Quaternion rotation, Vector2 direction, string team)
    {
        if(!hasAuthority)
            Arrow.FireArrow(spawn, rotation, direction, ArrowSpeed, Range, 0f, null, team); // Does not deal damage.
    }

    public void CallbackFireEnd()
    {
        InFire = false;
    }

    public override float GetNetworkSendInterval()
    {
        return 1f / SendRate;
    }
}
