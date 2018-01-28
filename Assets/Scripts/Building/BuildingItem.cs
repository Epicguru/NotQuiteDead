
using UnityEngine;

public class BuildingItem
{
    // Represents a building item that has a type, name, count, rarity and link with a real counterpart.

    public string Name; // UI name.
    public BuildingItemType Type; // Tile or furniture?
    public string Prefab; // For both tiles and furnitures.
    public int Count; // Since this is within an inventory, how many do we have?

    public BuildingItem(BaseTile tile, int count)
    {
        Type = BuildingItemType.TILE;
        Prefab = tile.Prefab;
        Name = tile.Name;
        Count = count;
    }

    public BuildingItem(Furniture furniture, int count)
    {
        Type = BuildingItemType.FURNITURE;
        Prefab = furniture.Prefab;
        Name = furniture.Name;
        Count = count;
    }

    public BaseTile GetTile()
    {
        if(Type != BuildingItemType.TILE)
        {
            Debug.LogError("This is not a tile! (" + Name + ", " + Prefab + ")");
            return null;
        }

        return BaseTile.GetTile(Prefab);
    }

    public Furniture GetFurniture()
    {
        if (Type != BuildingItemType.FURNITURE)
        {
            Debug.LogError("This is not furniture! (" + Name + ", " + Prefab + ")");
            return null;
        }

        return Furniture.GetFurniture(Prefab);
    }

    public ItemRarity GetRarity()
    {
        switch (Type)
        {
            case BuildingItemType.TILE:
                return GetTile().Rarity;

            case BuildingItemType.FURNITURE:
                return GetFurniture().Rarity;
            default:
                return ItemRarity.COMMON;
        }
    }

    public Sprite GetIcon()
    {
        switch (Type)
        {
            case BuildingItemType.TILE:
                return GetTile().Icon;

            case BuildingItemType.FURNITURE:
                return GetFurniture().Icon;
            default:
                return null;
        }
    }
}