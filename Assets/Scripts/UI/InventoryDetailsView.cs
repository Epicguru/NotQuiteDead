using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class InventoryDetailsView : MonoBehaviour
{
    public SpriteAtlas Atlas;
    public Image Image;
    public Text Title;
    public Text Text;

    public Sprite AccuracyIcon;
    public Sprite RarityIcon;
    public Sprite RPS_Icon;
    public Sprite DamageIcon;
    public Sprite DamageFalloffIcon;
    public Sprite PenetrationDamageFalloffIcon;
    public Sprite CapacityIcon;
    public Sprite PelletsPerShotIcon;
    public Sprite PenetrationIcon;
    public Sprite RangeIcon;
    public Sprite WeaponTypeIcon;
    public Sprite AttachmentTypeIcon;
    public Sprite AttachmentTweakIcon;

    public Transform Parent;
    public GameObject StatPrefab;
    public RectTransform StatContainer;

    private List<GameObject> spawned = new List<GameObject>();

    public void Enter(string prefab)
    {
        this.Enter(Item.GetItem(prefab));
    }

    public void Enter(Item item)
    {
        if (item == null)
            return;

        Debug.Log("Opening details for : " + item.Name);
        gameObject.SetActive(true);

        string desc = BuildDescription(item);
        
        Text.text = desc;
        Title.text = RichText.InColour(item.Name, ItemRarityUtils.GetColour(item.Rarity));

        SetStats(BuildStats(item));

        Sprite spr = Atlas.GetSprite(item.ItemIcon.name);
        Image.sprite = spr == null ? item.ItemIcon : spr;
    }

    private void SetStats(List<DetailStat> stats)
    {
        foreach(GameObject g in spawned)
        {
            Destroy(g);
        }
        spawned.Clear();

        if (stats == null)
            return;

        int i = 0;
        foreach(DetailStat s in stats)
        {
            GameObject g = Instantiate(StatPrefab, Parent);
            spawned.Add(g);
            g.GetComponent<DetailsStatistic>().Set(s);
            g.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -35 * i++ - 5);
        }

        int count = stats.Count;

        StatContainer.sizeDelta = new Vector2(0f, 10 + 35 * count);
    }

    private List<DetailStat> BuildStats(Item item)
    {
        List<DetailStat> stats = new List<DetailStat>();

        // Rarity, all items have this.
        stats.Add(new DetailStat { Icon = RarityIcon, Key = "InvDetails_Rarity".Translate(), Value = item.Rarity.ToString() });

        // Melee weapons damage stat.
        MeleeAttack melee = item.GetComponent<MeleeAttack>();
        if(melee != null)
        {
            stats.Add(new DetailStat { Icon = DamageIcon, Key = "InvDetails_MeleeDamage".Translate(), Value = melee.Damage.Damage.ToString() });
        }

        // Gun stats:
        Gun gun = item.GetComponent<Gun>();
        if(gun != null)
        {
            stats.Add(new DetailStat { Icon = WeaponTypeIcon, Key = "InvDetails_GunType".Translate(), Value = gun.GunType.ToString().Replace('_', ' ') });
            stats.Add(new DetailStat { Icon = DamageIcon, Key = "InvDetails_BaseDamage".Translate(), Value = gun.Shooting.Damage.Damage.ToString() + ((gun.Shooting.Capacity.BulletsPerShot.x > 1 || gun.Shooting.Capacity.BulletsPerShot.y > 1) ? (gun.Shooting.Damage.BulletsShareDamage ? " " + ("InvDetails_OverAllPellets".Translate()) : " " + ("InvDetails_PerPellet".Translate())) : "" )});
            if((gun.Shooting.Capacity.BulletsPerShot.x > 1 || gun.Shooting.Capacity.BulletsPerShot.y > 1))
            {
                if((gun.Shooting.Capacity.BulletsPerShot.x != gun.Shooting.Capacity.BulletsPerShot.y))
                {
                    stats.Add(new DetailStat { Icon = PelletsPerShotIcon, Key = "InvDetails_PPS".Translate(), Value = "InvDetails_PPS.Value".Translate(gun.Shooting.Capacity.BulletsPerShot.x, gun.Shooting.Capacity.BulletsPerShot.y) });
                }
                else
                {
                    stats.Add(new DetailStat { Icon = PelletsPerShotIcon, Key = "InvDetails_PPS".Translate(), Value = gun.Shooting.Capacity.BulletsPerShot.x.ToString() });
                }
            }
            stats.Add(new DetailStat { Icon = RangeIcon, Key = "InvDetails_Range".Translate(), Value = gun.Shooting.Damage.Range.ToString() + " m"});
            stats.Add(new DetailStat { Icon = CapacityIcon, Key = "InvDetails_MagCapacity".Translate(), Value = gun.Shooting.Capacity.MagazineCapacity.ToString() });
            stats.Add(new DetailStat { Icon = RPS_Icon, Key = "InvDetails_RoundsPerSec".Translate(), Value = gun.Shooting.GetRPS().ToString("n1") });
            if(gun.Shooting.Damage.Inaccuracy.x != gun.Shooting.Damage.Inaccuracy.y)
                stats.Add(new DetailStat { Icon = AccuracyIcon, Key = "InvDetails_Accuracy".Translate(), Value = "InvDetails_Accuracy.Val2".Translate(gun.Shooting.Damage.Inaccuracy.x.ToString(), gun.Shooting.Damage.Inaccuracy.y.ToString(), gun.Shooting.Damage.ShotsToInaccuracy.ToString()) });
            else
                stats.Add(new DetailStat { Icon = AccuracyIcon, Key = "InvDetails_Accuracy".Translate(), Value = "InvDetails_Accuracy.Value".Translate(gun.Shooting.Damage.Inaccuracy.x.ToString()) });

            if (gun.Shooting.Damage.DamageFalloff != 1f)
                stats.Add(new DetailStat { Icon = DamageFalloffIcon, Key = "InvDetails_DamageFalloff".Translate(), Value = "InvDetails_DF.Value".Translate(Mathf.FloorToInt(100f * (1f - gun.Shooting.Damage.DamageFalloff)).ToString()) });
            if(gun.Shooting.Damage.Penetration != 1)
            {
                stats.Add(new DetailStat { Icon = PenetrationIcon, Key = "InvDetails_Penetration".Translate(), Value = gun.Shooting.Damage.Penetration.ToString() });
                stats.Add(new DetailStat { Icon = PenetrationDamageFalloffIcon, Key = "InvDetails_PenFalloff".Translate(), Value = Mathf.CeilToInt(100f * (1f - gun.Shooting.Damage.PenetrationFalloff)) + "%" });
            }
        }

        // Attachment tweaks:
        Attachment a = item.GetComponent<Attachment>();
        if(a != null)
        {
            stats.Add(new DetailStat() { Icon = AttachmentTypeIcon, Key = "InvDetails_AttachmentSlot".Translate(), Value = a.Type.ToString().Replace('_', ' ') });
            foreach(AttachmentTweak t in item.GetComponentsInChildren<AttachmentTweak>())
            {
                stats.Add(new DetailStat() { Icon = AttachmentTweakIcon, Key = "InvDetails_Attribute".Translate(), Value = t.GetEffects() });
            }
        }

        return stats;
    }

    private string BuildDescription(Item item)
    {
        string description = "\n";

        description += RichText.InColour(RichText.InItalics(item.ShortDescription), Color.black) + "\n\n";
        description += RichText.InColour(item.LongDescription, Color.black) + "\n\n";

        return description;
    }

    public void Exit()
    {
        this.gameObject.SetActive(false);
    }
}
