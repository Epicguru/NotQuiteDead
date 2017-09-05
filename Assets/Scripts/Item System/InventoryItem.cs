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
    public Text Details;

    [Tooltip("The path to the prefab of this inventory item.")]
    public string ItemPrefab; // Used to find the item.

    [HideInInspector]
    public int ItemCount; // The number of items in this slot.

    [Tooltip("The inventory containing this item.")]
    public Inventory Inventory; // The inventory that 

    [Tooltip("The prefab version of this item. Not an instance.")]
    public Item Item; // The prefab version, not an instance!!!

    [Tooltip("The data for an item. Only used if there is one item in the stack.")]
    public ItemData Data;

    private static Vector2 StaticPos = new Vector2();

    public void Init(ItemData data)
    {
        // Nothing to do here, all rendering done in editor/game.

        // Get prefab
        Item = Item.FindItem(ItemPrefab);

        SetText();

        this.Data = data;

        Image.rectTransform.sizeDelta = new Vector2(Item.ItemIcon.texture.width > 200 ? 200 : Item.ItemIcon.texture.width, 29);

    }

    public void SetItemCount(int items)
    {
        if (ItemCount == items)
            return;

        ItemCount = items;

        if (ItemCount > 1)
            Data = null; // Should already be null...

        SetText();
    }

    public void SetText()
    {
        string quantity = ItemCount > 1 ? " x" + ItemCount : "";
        Text.text = Item.Name + quantity;
        Text.color = ItemRarityUtils.GetColour(Item.Rarity);
        Details.text = (Item.InventoryInfo.Weight * ItemCount) + "Kg";
    }

    public void Update()
    {
        StaticPos.y = (transform as RectTransform).anchoredPosition.y;
        (transform as RectTransform).anchoredPosition = StaticPos;
        Image.sprite = Item.ItemIcon;
    }

    public void Clicked()
    {
        //Debug.Log("Clicked!");
        Inventory.Options.Open(this);
    }
}