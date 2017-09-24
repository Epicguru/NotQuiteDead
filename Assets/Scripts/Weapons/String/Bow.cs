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

    private bool Drawing;
    public float P;
    public bool Released; // TODO Add minimum charge time!!!
    public bool InFire;

    public void Start()
    {
        Item = GetComponent<Item>();
        Animator = GetComponentInChildren<Animator>();
    }

    public void Update()
    {
        Animator.SetBool("Dropped", !Item.IsEquipped());
        Hand.SetEquipHandRender(transform, Item.IsEquipped());

        if (!Item.IsEquipped())
            return;

        if (!hasAuthority)
            return; // If not owner, stop here.

        UpdateDrawTime();

    }
    public void UpdateDrawTime()
    {
        if (InputManager.InputPressed("Aim") && !InFire)
        {
            P = Mathf.Clamp(P + Time.deltaTime * (1f / DrawTime), 0f, 1f);
            Released = false;
        }
        else
        {
            if(P == 1f && !Released)
            {
                DrawReleased(P);
                P = 0f;
                Released = true;
            }
            P = Mathf.Clamp(P - Time.deltaTime * (1f / DrawTime), 0f, 1f);
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

    public void DrawReleased(float force)
    {
        Animator.SetTrigger("Fire");
        InFire = true;
    }

    public void CallbackArrowFire()
    {
        Debug.Log("Pew!");
        Arrow.FireArrow(RealArrow.transform.position, RealArrow.transform.rotation, (InputManager.GetMousePos() - (Vector2)transform.parent.transform.position), ArrowSpeed, Range);
    }

    public void CallbackFireEnd()
    {
        InFire = false;
    }
}
