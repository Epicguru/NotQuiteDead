using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnim : MonoBehaviour {

    public bool Right;
    public bool Walking;
    public bool Running;
    public bool Dead;

    private float walkSpeed = 1.7f;
    private float runSpeed = 5f;
    private Animator anim;
    private Vector3 localScale = new Vector3(1, 1, 1);

	// Use this for initialization
	void Start () {
        anim = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
        anim.SetBool("Run", Running || Walking);
        anim.SetBool("Dead", Dead);
        anim.SetFloat("RunSpeed", Running ? runSpeed : walkSpeed);

        this.localScale.x = Right ? 1f : -1f;
        this.transform.localScale = this.localScale;
	}
}