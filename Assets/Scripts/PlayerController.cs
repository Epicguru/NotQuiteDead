using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    private PlayerAnim anim;
    private Hands hands;
    private Vector2 speed = new Vector2();

    private void Start()
    {
        anim = GetComponentInChildren<PlayerAnim>();
        hands = GetComponentInChildren<Hands>();
    }

    private void Update()
    {
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
            hands.Running = false;
        }
        else
        {
            anim.Walking = true;
            hands.Running = true;
            hands.RunSpeed = anim.Running ? 5f : 1.7f;
            hands.Right = anim.Right;
        }
        hands.Aiming = Input.GetMouseButton(1);

        if(hands.Aiming)
        {
            if (MouseToRight())
            {
                anim.Right = true;
                hands.Right = true;
            }
            else
            {
                hands.Right = false;
                anim.Right = false;
            }
        }

        speed.Normalize();
        speed *= realSpeed;

        this.GetComponent<Rigidbody2D>().velocity = speed;
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