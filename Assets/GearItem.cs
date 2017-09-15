using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GearItem : MonoBehaviour {

    public static Dictionary<string, GearItem> GearItems = new Dictionary<string, GearItem>();

    public string Title;
    public Text Text;
    public Image Image;

    public Item Item;

    public void Start()
    {
        SetItem(Item);

        GearItems.Add(Title, this);
    }

    public void SetItem(Item item)
    {
        // NOTE: Item is an INSTANCE!!!
        this.Item = item;
        if (item == null)
        {
            Text.text = Title;
            Image.sprite = null;
            Image.enabled = false;
        }
        else
        {
            Text.text = Title + " - " + item.Name;
            Image.sprite = item.ItemIcon;
            Image.enabled = true;
        }
    }

    public void Store(InventoryItem item)
    {
        // Store the current item.
        Player.Local.Holding.Item.RequestDataUpdate();
        Player.Local.Holding.CmdDrop(false, false, Player.Local.gameObject, Player.Local.Holding.Item.Data);
    }

    public void Drop(InventoryItem item)
    {
        // Drop the current item.
        Player.Local.Holding.Item.RequestDataUpdate();

        Player.Local.Holding.CmdDrop(true, false, Player.Local.gameObject, Player.Local.Holding.Item.Data);
    }

    public void Details(InventoryItem item)
    {
        // Show the details view.
        item.Inventory.DetailsView.Enter(item);
    }

    public ItemOption[] GetOptions(ItemData data)
    {
        List<ItemOption> options = new List<ItemOption>();
        options.Add(new ItemOption() { OptionName = "Store", OnSelected = Store });
        options.Add(new ItemOption() { OptionName = "Drop", OnSelected = Drop });
        options.Add(new ItemOption() { OptionName = "Details", OnSelected = Details });

        if (data == null)
            return options.ToArray();

        // Data stuff here, if applicable!

        return options.ToArray();
    }

    public void Clicked()
    {
        Debug.Log("Clicked!");

        // Quick Actions:
        if (InputManager.InputPressed("Quick Store", true))
        {
            // Wants to quick store, do it!
            Store(null);
            return;
        }

        if (InputManager.InputPressed("Quick Drop", true))
        {
            // Wants to quick drop, do it!
            Drop(null);
            return;
        }

        // Normal options...
        PlayerInventory.inv.Inventory.Options.Open(this);
    }
}
