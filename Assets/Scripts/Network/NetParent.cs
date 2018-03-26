using UnityEngine;
using UnityEngine.Networking;

public class NetParent : MonoBehaviour
{
    public string ID
    {
        get
        {
            return _ID;
        }
        set
        {
            _ID = value;
        }
    }
    [SerializeField]
    private string _ID = "Parent";

    public NetworkIdentity NetID
    {
        get
        {
            if (_NetID == null)
            {
                _NetID = GetComponentInParent<NetworkIdentity>();
                if (_NetID == null)
                {
                    Debug.LogError("There is not network ID on this game object or any of its parents! Invalid setup! ({0})".Form(gameObject.name));
                }
            }
            return _NetID;
        }
    }
    private NetworkIdentity _NetID;
}