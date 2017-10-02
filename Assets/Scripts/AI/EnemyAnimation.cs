using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimation : MonoBehaviour {

    public bool Walking;
    public float WalkSpeed = 1f;
    public bool Attacking;
    public float AttackSpeed = 1f;
    public bool Dead;

    public string WalkingParam = "Walking";
    public string WalkingSpeedParam = "WalkSpeed";
    public string DeadParam = "Dead";
    public string AttackParam = "Attack";
    public string AttackSpeedParam = "AttackSpeed";

    private Animator Animator;

    public void Start()
    {
        Animator = GetComponentInChildren<Animator>();
    }

    public void Update()
    {
        Animator.SetBool(WalkingParam, Walking);
        Animator.SetBool(DeadParam, Dead);
        Animator.SetFloat(WalkingSpeedParam, WalkSpeed);
        Animator.SetFloat(AttackSpeedParam, AttackSpeed);
        if (Attacking)
        {
            Animator.SetTrigger(AttackParam);
        }
        else
        {
            Animator.ResetTrigger(AttackParam);
        }

    }
}
