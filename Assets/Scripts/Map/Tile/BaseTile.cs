
public abstract class BaseTile : TileBackend
{
    public string Name; // TODO add ID or Prefab

    private TileLayer layer;

    public Chunk GetChunk()
    {
        return this.layer.GetChunkFromChunkCoords(GetChunkX(), GetChunkY());
    }

    public int GetChunkIndex()
    {
        return this.layer.GetChunkIndex(GetChunkX(), GetChunkY());
    }

    public int GetChunkX()
    {
        return this.GetX() / layer.ChunkSize;
    }

    public int GetChunkY()
    {
        return this.GetY() / layer.ChunkSize;
    }

    public TileLayer GetLayer()
    {
        return this.layer;
    }
}