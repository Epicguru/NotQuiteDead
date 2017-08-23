using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class InventoryDetailsView : MonoBehaviour
{
    public Image Image;
    public Text Title;
    public Text Text;

    public void Enter(InventoryItem item)
    {
        gameObject.SetActive(true);
        // Make description.

        string description = "";

        // Item first, static.

        // TODO - We need a system to keep data between inventory items and real items.
        // ATM the inventory item can only read prefab data, and not real data such as durability
        // bullets in gun etc.

        description += RichText.InBold("Rarity - ") + RichText.InColour(item.Item.Rarity.ToString(), ItemRarityUtils.GetColour(item.Item.Rarity)) + "\n";
        description += RichText.InBold("Weight - ") + item.Item.InventoryInfo.Weight + "Kg\n";
        if(item.Item.GetComponent<MeleeAttack>() != null)
        {
            description += RichText.InBold("Damage - ") + item.Item.GetComponent<MeleeAttack>().Damage.Damage + "\n";
        }

        description += "\n";

        description += RichText.InColour(RichText.InItalics(item.Item.Description.ShortDescription), Color.black) + "\n\n";
        description += RichText.InColour(item.Item.Description.LongDescription, Color.black) + "\n\n";

        Text.text = description;
        Title.text = item.Item.Name;
        Image.sprite = item.Item.ItemIcon;
    }

    public void Exit()
    {
        this.gameObject.SetActive(false);
    }
}
