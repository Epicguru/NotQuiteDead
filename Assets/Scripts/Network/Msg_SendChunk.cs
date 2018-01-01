using UnityEngine.Networking;

public class Msg_SendChunk : MessageBase
{
    public string Data;
    public int ChunkX, ChunkY;
}
