
using UnityEngine;

public struct BuildingItemData
{
    public string Name;
    public string Prefab;
    public int Count;
    public Sprite Icon;
    public ItemRarity Rarity;

    public Color GetColour()
    {
        return ItemRarityUtils.GetColour(Rarity);
    }
}