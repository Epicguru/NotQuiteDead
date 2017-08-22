using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour
{
    /*
    * Represents an item when it is inside an inventory. Simply has a reference to the item,
    * and some information about the item's location in the inventory.
    */
    public Text Text;
    public Image Image;
    public Dropdown Dropdown;
    public Text Details;

    [Tooltip("The path to the prefab of this inventory item.")]
    public string ItemPrefab; // Used to find the item.

    [Tooltip("The inventory containing this item.")]
    public Inventory Inventory; // The inventory that 

    [Tooltip("The prefab version of this item. Not an instance.")]
    public Item Item; // The prefab version, not an instance!!!

    private static Vector2 StaticPos = new Vector2();

    public void Init()
    {
        // Nothing to do here, all rendering done in editor/game.

        // Get prefab
        Item = Item.FindItem(ItemPrefab);

        Text.text = Item.Name;
        Text.color = ItemRarityUtils.GetColour(Item.Rarity);
        Details.text = Item.InventoryInfo.Weight + "Kg";

        GetComponentInChildren<AspectRatioFitter>().aspectRatio = Item.ItemIcon.texture.width / Item.ItemIcon.texture.height;
        Image.rectTransform.sizeDelta = new Vector2(0, 29);
    }

    public void Update()
    {
        StaticPos.y = (transform as RectTransform).anchoredPosition.y;
        (transform as RectTransform).anchoredPosition = StaticPos;
        Image.sprite = Item.ItemIcon;

        // HAHA this code :D
        const int MAX_WIDTH = 200;
        while (Image.rectTransform.sizeDelta.x > MAX_WIDTH)
        {
            Image.rectTransform.sizeDelta = new Vector2(0, Image.rectTransform.sizeDelta.y - 1);
        }
    }

    public void OptionSelected()
    {
        int option = Dropdown.value;
        switch (option)
        {
            case 0:
                // Ignore, for now...
                break;
            case 1:
                // Equip item: Yep. Thats it.
                Inventory.RemoveItem(this, false); // Remove, do not drop.
                Player.Local.Holding.CmdEquip(Item.Prefab, Player.Local.gameObject);
                break;
            case 2:
                // Drop item: for now just remove from inventory.
                Inventory.RemoveItem(this, true);
                break;
            case 3:
                // Item Details: Show the item details screen.
                Inventory.DetailsView.Enter(this);
                break;
            default:
                break;
        }

        // Change back to the 'default' option.
        Dropdown.value = 0;
    }
}