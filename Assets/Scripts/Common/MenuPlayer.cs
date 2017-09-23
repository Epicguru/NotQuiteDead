using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuPlayer : MonoBehaviour {

    public MenuPlayerState State;
    public Animator PlayerAnim;
    public Animator GunAnim;
    public GunAnimation GunAnimScript;

    public void Update()
    {
        switch (State)
        {
            case MenuPlayerState.IDLE:

                PlayerAnim.SetBool("Walking", false);
                PlayerAnim.SetFloat("WalkSpeed", 1f);

                GunAnim.SetBool(GunAnimScript.Run, false);
                GunAnim.SetBool(GunAnimScript.Aim, false);
                GunAnim.SetBool(GunAnimScript.Shoot, false);

                break;
            case MenuPlayerState.SHOOTING:

                PlayerAnim.SetBool("Walking", false);
                PlayerAnim.SetFloat("WalkSpeed", 1f);

                GunAnim.SetBool(GunAnimScript.Run, false);
                GunAnim.SetBool(GunAnimScript.Aim, true);
                GunAnim.SetBool(GunAnimScript.Shoot, true);

                break;
            case MenuPlayerState.WALKING:

                PlayerAnim.SetBool("Walking", true);
                PlayerAnim.SetFloat("WalkSpeed", 1f);

                GunAnim.SetBool(GunAnimScript.Run, true);
                GunAnim.SetBool(GunAnimScript.Aim, false);
                GunAnim.SetBool(GunAnimScript.Shoot, false);

                break;
            default:

                break;
        }
    }
}

public enum MenuPlayerState
{
    IDLE,
    WALKING,
    SHOOTING
}
