using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class BlueprintRequirement : MonoBehaviour
{
    public static SpriteAtlas Atlas;

    public bool InInventory;
    public Text Text;
    public Image Image;
    public Workbench Workbench;
    public Image Redout;

    public Item Item
    {
        get
        {
            return _Item;
        }
        set
        {
            if (value == _Item)
                return;
            _Item = value;
            RefreshItem();
        }
    }
    private Item _Item;
    public int Amount
    {
        get
        {
            return _Amount;
        }
        set
        {
            if (value == _Amount)
                return;
            _Amount = value;
            RefreshAmount();
        }
    }
    private int _Amount;

    public void RefreshItem()
    {
        if(Item == null)
        {
            Image.enabled = false;
            RefreshAmount();
            return;
        }

        Image.enabled = true;
        if (Atlas == null)
            Atlas = Resources.Load<SpriteAtlas>("Atlas/Game Point");
        Sprite spr = Atlas.GetSprite(Item.ItemIcon.name);
        Image.sprite = spr == null ? Item.ItemIcon : spr;
        RefreshAmount();
    }

    public void RefreshAmount()
    {
        Text.text = RichText.InBold(RichText.InColour(Item == null ? "---" : Item.Name, Item == null ? Color.black : ItemRarityUtils.GetColour(Item.Rarity))) + " x" + Amount;
    }

    public void Clicked()
    {
        if (Workbench == null)
            Workbench = GetComponentInParent<Workbench>();

        Workbench.Preview.Item = this.Item;
    }

    public void Update()
    {
        Redout.enabled = !InInventory;
    }
}
