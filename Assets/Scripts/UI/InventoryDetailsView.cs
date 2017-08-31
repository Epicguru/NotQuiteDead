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
        Debug.Log("Opening details for : " + item.Item.Name);
        gameObject.SetActive(true);
        // Make description.

        string description = "";

        // Item first, static.

        // TODO - We need a system to keep data between inventory items and real items.
        // ATM the inventory item can only read prefab data, and not real data such as durability
        // bullets in gun etc.

        description += RichText.InBold("Rarity - ") + RichText.InColour(item.Item.Rarity.ToString(), ItemRarityUtils.GetColour(item.Item.Rarity)) + "\n";
        description += RichText.InBold("Weight - ") + item.Item.InventoryInfo.Weight + "Kg\n";
        if(item.Item.GetComponent<Weapon>() != null)
        {
            description += RichText.InBold("Weapon Type - ") + item.Item.GetComponent<Weapon>().Type + "\n";
        }
        if(item.Item.GetComponent<MeleeAttack>() != null)
        {
            description += RichText.InBold("Damage - ") + item.Item.GetComponent<MeleeAttack>().Damage.Damage + "\n";
        }
        Gun gun = item.Item.GetComponent<Gun>();
        if(gun != null)
        {
            GunDamage damage = gun.GetComponent<GunShooting>().Damage;
            GunCapacity capacity = gun.GetComponent<GunShooting>().Capacity;
            description += RichText.InBold("Gun Type - ") + gun.GunType.ToString().Replace('_', ' ') + "\n";
            description += RichText.InBold("Damage - ") + damage.Damage;
            if (damage.BulletsShareDamage && capacity.BulletsPerShot.y > 1)
            {
                description += "total over " + capacity.BulletsPerShot.x + " to " + capacity.BulletsPerShot.y + " projectiles per shot";
            }
            description += "\n";
            description += RichText.InBold("Range - ") + damage.Range + "\n";
            description += RichText.InBold("Magazine Capacity - ") + capacity.MagazineCapacity + "\n";
            description += RichText.InBold("Best Accuracy - ") + damage.Inaccuracy.x + "°\n";
            description += RichText.InBold("Worst Accuracy - ") + damage.Inaccuracy.y + "°\n";
            description += RichText.InBold("Shots Before Worst Accuracy - ") + damage.ShotsToInaccuracy + "\n";
            description += RichText.InBold("Accuracy Reset - ") + (int)(damage.ShotsInaccuracyCooldown * 100f) + "% per second\n";
            description += RichText.InBold("Penetration - ") + damage.Penetration + "\n";
            description += RichText.InBold("Penetration Falloff - ") + (int)(damage.PenetrationFalloff * 100f) + "%\n";
            description += RichText.InBold("Damage Falloff - ") + (int)(damage.DamageFalloff * 100f) + "%\n";
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
