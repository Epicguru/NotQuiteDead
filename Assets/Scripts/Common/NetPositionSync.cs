using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetPositionSync : NetworkBehaviour {

    public float SendRate = 20f;
    public bool Extrapolate = true;
    [SyncVar(hook = "NewPosition")]
    [HideInInspector]
    public Vector3 Position;
    [HideInInspector]
    public Vector3 OldPosition;
    [HideInInspector]
    public Vector3 Velocity;
    [HideInInspector]
    public Vector3 LastSentPosition;

    private float timer;
    private float timeSinceReceived;

    public void Start()
    {
        OldPosition = Position;
    }

    public void Update()
    {
        if (isServer)
            UpdateSending();
        UpdatePosition();
    }

    public void UpdateSending()
    {
        timer += Time.deltaTime;
        if (timer >= GetNetworkSendInterval())
        {
            timer -= GetNetworkSendInterval();

            // Make sure not to send the same position.
            if (transform.position == LastSentPosition)
                return;
            LastSentPosition = transform.position;

            if (isServer)
            {
                // Just set the variable
                Velocity = transform.position - Position;
                Velocity /= GetNetworkSendInterval();
                Position = transform.position;
            }
            else
            {
                // Need to send a CMD. Use the local player to set.
                // EDIT - Not currently in use.
                Player.Local.NetUtils.CmdSetPositionSync(gameObject, transform.position);

            }
        }
    }

    public void UpdatePosition()
    {
        if (!isServer)
        {
            timeSinceReceived += Time.deltaTime;
            //transform.position = GetInterpolatedPosition(!Extrapolate);
            transform.position = GetInterpolatedPosition(true); // EDIT - Extrapolation is buggy AF TODO FIXME
        }
    }

    public Vector3 GetInterpolatedPosition(bool clamp)
    {
        float p = GetLerpPercentage(clamp);

        Vector3 extrapolated = Vector3.LerpUnclamped(OldPosition, Position, p);

        return extrapolated;
    }

    public float GetLerpPercentage(bool clamp)
    {
        float p = timeSinceReceived / GetNetworkSendInterval();

        if (clamp)
            p = Mathf.Clamp(p, 0f, 1f);

        return p;
    }

    public void NewPosition(Vector3 pos)
    {
        if (isServer)
            return; // Dont care about this.

        this.OldPosition = Position;
        this.Position = pos;
        timeSinceReceived = 0f;
        Velocity = Position - OldPosition;
        Velocity /= GetNetworkSendInterval();
    }

    public override float GetNetworkSendInterval()
    {
        return 1f / SendRate;
    }
}
