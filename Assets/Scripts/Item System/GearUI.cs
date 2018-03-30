using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GearUI : MonoBehaviour {

    public static Dictionary<string, GearUI> GearItems = new Dictionary<string, GearUI>();

    public string Slot;
    public Text Text;
    public Image Image;
    public bool Hands = false;

    public Item Item;

    public void Start()
    {
        SetItem(Item);

        // Needs to be added to dictionary in a static script.
    }

    public void SetItem(Item item)
    {
        // NOTE: Item is an INSTANCE!!!
        this.Item = item;
        if (item == null)
        {
            Text.text = ("GearSlot_" + Slot).Translate();
            Image.sprite = null;
            Image.enabled = false;
        }
        else
        {
            Text.text = ("GearSlot_" + Slot).Translate() + " - " + item.Name;
            Image.sprite = item.ItemIcon;
            Image.enabled = true;
        }
    }

    public void Store(ItemStack item)
    {
        if (Hands)
        {
            // Store the current item.
            Player.Local.Holding.Item.RequestDataUpdate();
            Player.Local.Holding.CmdDrop(false, false, Player.Local.gameObject, Player.Local.Holding.Item.Data.Serialize());
        }
        else
        {
            // Remove from slot noramlly.
            Player.Local.GearMap[Slot].GetGearItem().Item.RequestDataUpdate();
            Player.Local.NetUtils.CmdSetGear(Slot, null, null, true);
        }
    }

    public void Drop(ItemStack item)
    {
        if (Hands)
        {
            // Drop the current item.
            Player.Local.Holding.Item.RequestDataUpdate();
            if (Player.Local.Holding.Item.Data != null)
                Player.Local.Holding.Item.Data.Update("Quick Slot", 0);

            Player.Local.Holding.CmdDrop(true, false, Player.Local.gameObject, Player.Local.Holding.Item.Data.Serialize());
        }
        else
        {
            // Remove from slot and drop on ground.
            BodyGear g = Player.Local.GearMap[Slot];

            if(g.GetGearItem() == null)
            {
                Debug.LogError("Cannot drop, looks like the UI and world are desynced!");
                return;
            }

            g.GetGearItem().Item.RequestDataUpdate();
            ItemData data = g.GetGearItem().Item.Data;
            Player.Local.NetUtils.CmdDropGear(Slot, data.Serialize());
        }
    }

    public void Details(ItemStack stack)
    {
        // Show the details view.
        if (stack != null)
            PlayerInventory.inv.Inventory.DetailsView.Enter(stack.Prefab);
        else
            PlayerInventory.inv.Inventory.DetailsView.Enter(Item.GetItem(stack.Prefab));
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
        if (Item == null)
            return;

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
