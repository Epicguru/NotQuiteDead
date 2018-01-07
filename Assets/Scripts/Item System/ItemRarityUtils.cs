using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ItemRarityUtils : MonoBehaviour
{
    public Color Rubbish;
    public Color Common;
    public Color Valuable;
    public Color Rare;
    public Color Epic;
    public Color Legendary;
    public Color Godlike;

    private static ItemRarityUtils instance;

    public void Awake()
    {
        // TODO WHY HERE :D
        Item.LoadItems();
        Furniture.LoadAllFurniture();
        // TODO move both things to a better place.
        BaseTile.LoadPrefabs();
        instance = this;
    }

    public void Start()
    {
        Item.RegisterItems();
        Furniture.RegisterAll();
    }

    public static string GetDescription(ItemRarity rarity)
    {
        switch (rarity)
        {
            case ItemRarity.RUBBISH:
                return "A useless item, quite literally a waste of space and thought.";
            case ItemRarity.COMMON:
                return "A common and standard object, that is easily replaced.";
            case ItemRarity.VALUABLE:
                return "A vaulable item, often not replaceable.";
            case ItemRarity.RARE:
                return "A rare item that is highly valuable and quite irreplaceable.";
            case ItemRarity.EPIC:
                return "A superior object that is extremely rare and valuable.";
            case ItemRarity.LEGENDARY:
                return "A legendary object that was thought to only exist in ancient fables.";
            case ItemRarity.GODLIKE:
                return "A item so powerful and valuable that some say it was crafted eons ago by greater beings.";
            default:
                return "UKNOWN";
        }
    }

    public static Color GetColour(ItemRarity rarity)
    {
        switch (rarity)
        {
            case ItemRarity.RUBBISH:
                return instance.Rubbish;
            case ItemRarity.COMMON:
                return instance.Common;
            case ItemRarity.VALUABLE:
                return instance.Valuable;
            case ItemRarity.RARE:
                return instance.Rare;
            case ItemRarity.EPIC:
                return instance.Epic;
            case ItemRarity.LEGENDARY:
                return instance.Legendary;
            case ItemRarity.GODLIKE:
                return instance.Godlike;
            default:
                return Color.white;
        }
    }
}

