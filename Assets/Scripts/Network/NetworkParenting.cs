using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public class NetworkParenting : NetworkBehaviour
{
    public bool IsParented
    {
        get
        {
            return parentID != NetworkInstanceId.Invalid && !parentID.IsEmpty();
        }
    }

    [SyncVar(hook = "UpdateNetParent")]
    private NetworkInstanceId parentID;

    public override void OnStartClient()
    {
        UpdateNetParent(parentID);
    }

    [Server]
    public void SetParent(Transform trans)
    {
        if (trans != null)
        {
            // Check to make sure this object is not already attached to that parent.
            if (trans == transform.parent)
            {
                return;
            }

            // Get the network ID: This parent must be a networked object.
            var ni = trans.GetComponent<NetworkIdentity>();
            if (ni == null)
            {
                Debug.LogError("The target transform to parent to does not have a NetworkIdentity attached to it! The parent will not be changed.");
                return;
            }
            parentID = ni.netId;
            transform.SetParent(trans);
        }
        else
        {
            if (parentID != NetworkInstanceId.Invalid)
            {
                parentID = NetworkInstanceId.Invalid;
            }
            transform.SetParent(null);
        }
    }

    private void UpdateNetParent(NetworkInstanceId newID)
    {
        this.parentID = newID;

        if (IsParented)
        {
            GameObject go = ClientScene.FindLocalObject(parentID);
            if (go == null)
            {
                Debug.LogError("This object is parented, but the ClientScene could not find the local object!");
            }
            else
            {
                transform.SetParent(go.transform);
            }
        }
        else
        {
            transform.SetParent(null);
        }
    }
}