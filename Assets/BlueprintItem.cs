using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class BlueprintItem : MonoBehaviour
{
    public static SpriteAtlas Atlas;

    public Workbench Workbench;
    public bool CanCraft = true;

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
            Refresh();
        }
    }

    public Text Text;
    public Image Image;
    public Image Redout;

    public Blueprint Blueprint;

    private Item _Item;

    public void Refresh()
    {
        if(Item == null)
        {
            Text.text = "ERROR";
            Image.enabled = false;
        }
        else
        {
            Text.text = RichText.InColour(Item.Name, ItemRarityUtils.GetColour(Item.Rarity));
            if (Atlas == null)
                Atlas = Resources.Load<SpriteAtlas>("Atlas/Game Point");
            Sprite spr = Atlas.GetSprite(Item.ItemIcon.name);
            Image.sprite = spr == null ? Item.ItemIcon : spr;
        }
    }

    public void Update()
    {
        Redout.enabled = !CanCraft;
    }

    public void Clicked()
    {
        if (Workbench == null)
            Workbench = GetComponentInParent<Workbench>();
        Workbench.SetCurrentBlueprint(Blueprint);
        Workbench.Preview.Item = Item;
        Workbench.Requirements.CurrentBlueprint = Blueprint;
        Workbench.Results.CurrentBlueprint = Blueprint;
    }
}
