
using UnityEngine.Networking;

public enum MessageTypes : short
{
    SEND_CHUNK_DATA = MsgType.Highest + 1,
    SEND_TILE_CHANGE = MsgType.Highest + 2
}