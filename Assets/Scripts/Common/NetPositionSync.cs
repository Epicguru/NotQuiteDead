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

    public void Update()
    {
        if (hasAuthority)
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
                Player.Local.NetUtils.CmdSetPositionSync(gameObject, transform.position);

            }
        }
    }

    public void UpdatePosition()
    {
        if (!hasAuthority)
        {
            timeSinceReceived += Time.deltaTime;
            transform.position = GetExtrapolatedPosition(false);
        }
    }

    public Vector3 GetExtrapolatedPosition(bool clamp)
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
        if (hasAuthority)
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
