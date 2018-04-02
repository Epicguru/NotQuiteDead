using UnityEngine;

[CreateAssetMenu(fileName = "TileName", menuName = "Tiles/Base Map Tile", order = 2)]
public class BaseMapTile : BaseTile
{
    public override int GetSpriteIndex(int left, int top, int right, int bottom)
    {
        if (bottom == 0)
            return 1;
        return 0;
    }
}