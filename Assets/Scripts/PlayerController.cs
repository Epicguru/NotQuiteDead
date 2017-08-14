using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public Transform CameraTarget;

    private PlayerAnim anim;
    private GunManager weapons;
    private Vector2 speed = new Vector2();

    private void Start()
    {
        anim = GetComponentInChildren<PlayerAnim>();
        weapons = GetComponentInChildren<GunManager>();
    }

    private void Update()
    {
        this.PlaceCameraTarget();

        float realSpeed = 3;
        anim.Running = false;
        if (Input.GetKey(KeyCode.LeftShift))
        {
            realSpeed = 7f;
            anim.Running = true;
        }

        speed.Set(0, 0);
        if (Input.GetKey(KeyCode.A))
        {
            speed.x -= 1;
            anim.Right = false;
        }
        if (Input.GetKey(KeyCode.D))
        {
            speed.x += 1;
            anim.Right = true;
        }
        if (Input.GetKey(KeyCode.S))
        {
            speed.y -= 1;
        }
        if (Input.GetKey(KeyCode.W))
        {
            speed.y += 1;
        }

        if(speed.x == 0f && speed.y == 0f)
        {
            anim.Walking = false;
            anim.Running = false;
            weapons.Running = false;
        }
        else
        {
            anim.Walking = true;
            weapons.Running = true;
            weapons.RunSpeed = anim.Running ? 5f : 1.7f;
            weapons.Right = anim.Right;
        }
        weapons.Aiming = Input.GetMouseButton(1);

        if(weapons.Aiming)
        {
            if (MouseToRight())
            {
                anim.Right = true;
                weapons.Right = true;
            }
            else
            {
                weapons.Right = false;
                anim.Right = false;
            }
        }

        speed.Normalize();
        speed *= realSpeed;

        this.GetComponent<Rigidbody2D>().velocity = speed;
    }

    public void PlaceCameraTarget()
    {
        CameraTarget.transform.localPosition = Vector3.zero;
    }

    public bool MouseToRight()
    {
        Vector3 v3 = Input.mousePosition;
        v3.z = 0;
        v3 = Camera.main.ScreenToWorldPoint(v3);
        float dstX = transform.position.x - v3.x;


        return dstX <= 0;
    }

}