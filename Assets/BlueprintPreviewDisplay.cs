
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class BlueprintPreviewDisplay : MonoBehaviour
{
    public SpriteAtlas Atlas;
    public string DefaultTitle;
    public string DefaultDescription;

    public Text Title;
    public Text Description;
    public Image Image;

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
    private Item _Item;

    public void Refresh()
    {
        if(Item == null)
        {
            Title.text = DefaultTitle;
            Description.text = DefaultDescription;
            Image.enabled = false;
        }
        else
        {
            Image.enabled = true;
            Sprite spr = Atlas.GetSprite(Item.ItemIcon.name);
            Image.sprite = spr == null ? Item.ItemIcon : spr;
            Title.text = Item.Name;
            Description.text = "";
            Description.text += RichText.InBold(RichText.InItalics(Item.Description.ShortDescription)) + "\n";
            Description.text += Item.Description.LongDescription;
        }
    }

    public void Update()
    {
        Refresh();
    }
}