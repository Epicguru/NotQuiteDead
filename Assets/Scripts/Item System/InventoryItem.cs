using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour
{
    public static SpriteAtlas Atlas;
    /*
    * Represents an item when it is inside an inventory. Simply has a reference to the item,
    * and some information about the item's location in the inventory.
    */
    public Text Text;
    public Image Image;
    public Text Details;
    public bool Resize = true;

    public InventoryItemData ItemData;

    public void Init(ItemData data)
    {
        // Nothing to do here, all rendering done in editor/game.
        SetText();

        this.ItemData.Data = data;

        if(Resize)
            Image.rectTransform.sizeDelta = new Vector2(ItemData.Item.ItemIcon.texture.width > 200 ? 200 : ItemData.Item.ItemIcon.texture.width, 29);
    }

    public void SetText()
    {
        string quantity = ItemData.Count > 1 ? " x" + ItemData.Count : "";
        string quickSlot = "";
        if (ItemData.Data != null && ItemData.Data.QuickSlot != 0)
            quickSlot = RichText.InColour(" (Slot #" + ItemData.Data.QuickSlot + ")", Color.black);
        Text.text = ItemData.Item.Name + quantity + quickSlot;
        Text.color = Color.Lerp(ItemRarityUtils.GetColour(ItemData.Item.Rarity), Color.black, 0.2f);
        if(Details != null)
            Details.text = (ItemData.Item.InventoryInfo.Weight * ItemData.Count) + "Kg";
        if (Atlas == null)
            Atlas = Resources.Load<SpriteAtlas>("Atlas/Game Point");
        Sprite spr = Atlas.GetSprite(ItemData.Item.ItemIcon.name);
        Image.sprite = spr == null ? ItemData.Item.ItemIcon : spr;
    }

    public void Clicked()
    {
        // Check quick options:
        if(((ItemData.Item.Equipable && !Player.Local.Building.InBuildMode) || ItemData.Item.GetComponent<GearItem>() != null) && InputManager.InputPressed("Quick Equip", true))
        {
            // Wants to quick equip, do it!
            if (ItemData.Item.GetComponent<GearItem>() != null)
                Item.Option_EquipGear(this.ItemData, this.ItemData.Prefab);
            else
                Item.Option_Equip(this.ItemData, this.ItemData.Prefab);
            return;
        }

        if (InputManager.InputPressed("Quick Drop", true))
        {
            // Wants to quick drop, do it!
            Item.Option_Drop(this.ItemData, this.ItemData.Prefab);
            return;
        }

        PlayerInventory.inv.Inventory.Options.Open(this);
    }
}