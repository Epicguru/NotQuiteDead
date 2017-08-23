using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerMovement : NetworkBehaviour
{
    public float WalkSpeed = 3.5f;
    public float RunSpeed = 6.5f;

    private new Rigidbody2D rigidbody;
    private new PlayerAnimation animation;
    private PlayerDirection direction;
    private Vector2 velocity = new Vector2();

    public void Start()
    {
        animation = GetComponent<PlayerAnimation>();
        rigidbody = GetComponent<Rigidbody2D>();
        direction = GetComponent<PlayerDirection>();
        rigidbody.velocity = Vector2.zero;
        velocity = Vector2.zero;
    }

    public void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        MovePlayer();
    }

    [Client]
    private void MovePlayer()
    {
        // Get inputs
        velocity.Set(0, 0);

        if (InputManager.InputPressed("Up"))
        {
            velocity.y += 1;
        }
        if (InputManager.InputPressed("Down"))
        {
            velocity.y -= 1;
        }
        if (InputManager.InputPressed("Right"))
        {
            velocity.x += 1;
            direction.Right = true;
        }
        if (InputManager.InputPressed("Left"))
        {
            velocity.x -= 1;
            direction.Right = false;
        }
        bool sprinting = false;
        if (InputManager.InputPressed("Sprint"))
            sprinting = true;

        velocity.Normalize();
        velocity *= sprinting ? RunSpeed : WalkSpeed;

        rigidbody.velocity = velocity;

        CmdAnimatePlayer(velocity != Vector2.zero, sprinting);
    }

    [Command]
    public void CmdAnimatePlayer(bool isMoving, bool isRunning)
    {
        animation.Moving = isMoving;
        animation.Running = isRunning;
    }
}
