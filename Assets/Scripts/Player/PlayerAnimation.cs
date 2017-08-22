using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[NetworkSettings(sendInterval = 0.05f)]
public class PlayerAnimation : NetworkBehaviour {

    [SyncVar]
    public bool Moving;
    [SyncVar]
    public bool Running;

    private Animator animator;

    public void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }

    public void Update()
    {
        SetStates();
    }

    public void SetStates()
    {
        animator.SetBool("Walking", Moving);
        animator.SetFloat("WalkSpeed", Running ? 2 : 1);
    }
}
