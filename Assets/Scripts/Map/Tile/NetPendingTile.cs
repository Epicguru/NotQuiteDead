
public struct NetPendingTile
{
    public string Prefab;
    public int X;
    public int Y;

    public bool PrefabIsNull()
    {
        return string.IsNullOrEmpty(Prefab);
    }
}