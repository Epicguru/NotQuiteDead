using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetPosSync : NetworkBehaviour
{
    // Synchronises the position of an object across all clients and also on the server.
    // The server always has the authorative position:
    // TODO implement snapping back to last known pos.

    [Header("Global")]
    [Range(0f, 60f)]
    public float UpdatesPerSecond = 40f;
    public QosType Channel = QosType.Unreliable;

    [System.Serializable]
    private class Pos
    {
        [Range(0.1f, 100f)]
        public float LerpSpeed = 25f;
        public bool Lerp = true;
    }
    [Header("Controls")]
    [SerializeField]
    private Pos _Position;

    [System.Serializable]
    private class Rot
    {
        public bool Sync = false;
        public bool Lerp = true;
        [Range(0.1f, 100f)]
        public float LerpSpeed = 25f;
    }
    [SerializeField]
    private Rot _Rotation;

    [System.Serializable]
    private class Parent
    {
        public bool Sync
        {
            get
            {
                return true;
            }
        }
    }
    [SerializeField]
    private Parent _Parent;

    [Header("Debug")]
    [SyncVar]
    [ReadOnly]
    public Vector2 Position;

    [SyncVar]
    [ReadOnly]
    public float Angle;

    [SyncVar]
    [ReadOnly]
    public NetworkInstanceId ParentID;

    [SyncVar(hook = "NewParent")]
    [ReadOnly]
    public string ParentName;

    public bool IsParented
    {
        get
        {
            return ParentID != NetworkInstanceId.Invalid && !ParentID.IsEmpty() && !string.IsNullOrEmpty(ParentName);
        }
    }

    private bool dirty;

    public override void OnStartClient()
    {
        if (isServer)
        {
            // This might be called on a host: a client and a server in the same process.
            return;
        }

        transform.localPosition = Position;
        if (_Rotation.Sync)
        {
            transform.localEulerAngles = new Vector3(0f, 0f, Angle);
        }
        if (_Parent.Sync)
        {
            NewParent(ParentName);
        }

    }

    public void Update()
    {
        if (isClient && !isServer)
        {
            LerpToPosition();
            LerpToAngle();
        }
        else
        {
            UpdateDirty();
            UpdateSending();
        }
    }

    [Server]
    public void SetParent(NetParent parent)
    {
        if (!_Parent.Sync)
        {
            Debug.LogWarning("Parent synchronization is disabled for {0}, parenting in local scene only...".Form(gameObject.name));
            transform.SetParent(parent == null ? null : parent.transform);
        }
        else
        {
            if (parent == null)
            {
                if (!IsParented)
                {
                    // Requested to unparent, but already unparented!
                    if (transform.parent != null)
                    {
                        transform.SetParent(null);
                    }
                    return;
                }
                else
                {
                    // This will sync with the clients.
                    ParentID = NetworkInstanceId.Invalid;
                    ParentName = "";
                    if (transform.parent != null)
                    {
                        transform.SetParent(null);
                    }
                }
            }
            else
            {
                if (parent.NetID == null)
                {
                    Debug.LogWarning("Invalid NetParent setup, cannot parent.");
                    return;
                }
                NetworkInstanceId id = parent.NetID.netId;
                string name = parent.ID;

                if (id == ParentID)
                {
                    if (name == ParentName)
                    {
                        // Already parented to the object.
                        return;
                    }
                }

                // Apply to syncvars.
                ParentID = id;
                ParentName = name;
                transform.SetParent(parent.transform);
            }
        }
    }

    [ClientCallback]
    private void NewParent(string name)
    {
        if (isServer)
            return;

        this.ParentName = name;

        // The ID should have already updated beforehand.
        if (!string.IsNullOrEmpty(name) && !IsParented)
        {
            Debug.LogError("Should be attached to '{0}' but the net ID is invalid or empty! Cannot parent!".Form(name));
            return;
        }

        if (IsParented)
        {
            var go = ClientScene.FindLocalObject(ParentID);
            if (go == null)
            {
                Debug.LogError("Scene ID is valid, but returned no game object?!? Cannot parent!");
                return;
            }
            else
            {
                // Now find the NetParent that should be a child of the game object.
                NetParent[] parents = go.GetComponentsInChildren<NetParent>();
                foreach (var parent in parents)
                {
                    if (parent.ID == name)
                    {
                        // Found the correct parent, apply!
                        transform.SetParent(parent.transform);
                        return;
                    }
                }
                // IF we get to here the parent was not found...
                Debug.LogError("NetParent '{0}' was not found on networked object '{1}'! Cannot parent!".Form(name, go.name));
                return;
            }
        }
        else
        {
            // Just unparent from everything.
            transform.SetParent(null);
        }
    }

    [Server]
    public void UpdateDirty()
    {
        if ((Vector2)transform.localPosition != Position)
        {
            dirty = true;
        }
        if (!dirty)
        {
            if (transform.localEulerAngles.z != Angle)
            {
                dirty = true;
            }
        }
    }

    [Server]
    public void UpdateSending()
    {
        if (dirty)
        {
            Position = transform.localPosition;
            if (_Rotation.Sync)
            {
                Angle = transform.localEulerAngles.z;
            }
            dirty = false;
        }
    }

    [Client]
    public void LerpToPosition()
    {
        if (_Position.Lerp)
        {
            transform.localPosition = Vector2.Lerp(transform.localPosition, Position, Time.deltaTime * _Position.LerpSpeed);
        }
        else
        {
            transform.localPosition = Position;
        }
    }

    [Client]
    public void LerpToAngle()
    {
        if (!_Rotation.Sync)
        {
            return;
        }

        if (_Rotation.Lerp)
        {
            Vector3 euler = transform.localEulerAngles;
            euler.z = Mathf.LerpAngle(euler.z, Angle, Time.deltaTime * _Rotation.LerpSpeed);
            transform.localEulerAngles = euler;
        }
        else
        {
            Vector3 euler = transform.localEulerAngles;
            euler.z = Angle;
            transform.localEulerAngles = euler;
        }
    }

    public override float GetNetworkSendInterval()
    {
        if (UpdatesPerSecond == 0f)
        {
            return 0f;
        }
        return 1f / UpdatesPerSecond;
    }

    public override int GetNetworkChannel()
    {
        return (int)Channel;
    }
}