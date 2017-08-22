using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerHeadLook : NetworkBehaviour {

    public Transform Neck;
    public bool Active = true;
    public float SendRate = 2f; // Times per second.
    public bool Interpoalte = true;

    private Vector2 myPos = new Vector2();
    private PlayerDirection dir;

    [SyncVar(hook = "NewNeckAngle")]
    private float NeckAngle;

    private float oldAngle = 0;
    private float timer;

    public void Start()
    {
        dir = GetComponent<PlayerDirection>();
    }

	public void LateUpdate()
    {
        if (isLocalPlayer)
            CalculateAngle();
        UpdateRotation();
    }

    public void NewNeckAngle(float newAngle)
    {
        oldAngle = NeckAngle;
        NeckAngle = newAngle;
        timer = 0;
    }

    public void CalculateAngle()
    {
        if (!Active)
        {
            CmdSetAngle(0);
            return;
        }
        Vector2 mousePos = InputManager.GetMousePos();
        myPos.Set(transform.position.x, transform.position.y);

        Vector2 dst = mousePos - myPos;

        if (!dir.Right)
        {
            dst.x = -dst.x;
        }

        float angle = Mathf.Atan2(dst.y, dst.x) * Mathf.Rad2Deg;

        angle *= 0.5f;
        CmdSetAngle(angle);
    }

    [Command]
    public void CmdSetAngle(float angle)
    {
        this.NeckAngle = angle;
    }

    public float GetLerpedAngle()
    {
        timer += Time.deltaTime;

        float p = timer / GetNetworkSendInterval();

        if (p > 1)
            p = 1;

        return Mathf.LerpAngle(oldAngle, NeckAngle, p);
    }

    public void UpdateRotation()
    {
        Neck.localRotation = Quaternion.Euler(0, 0, GetLerpedAngle());
    }

    public override float GetNetworkSendInterval()
    {
        return 1f / this.SendRate;
    }
}
