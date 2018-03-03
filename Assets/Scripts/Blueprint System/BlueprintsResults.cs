using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueprintsResults : MonoBehaviour
{
    public BlueprintRequirement Prefab;
    public Workbench Workbench;
    public Transform Parent;
    public List<BlueprintRequirement> Spawned = new List<BlueprintRequirement>();

    public void CraftButton()
    {
        // Assume that the button is not pressed when crafting is not available.

        Blueprint b = Workbench.CurrentBlueprint;

        for (int i = 0; i < b.Requirements.Length; i++)
        {
            PlayerInventory.Remove(b.Requirements[i].Prefab, Vector2.zero, false, b.RequirementQuantities[i]);
        }

        for (int i = 0; i < b.Products.Length; i++)
        {
            PlayerInventory.Add(b.Products[i].Prefab, null, b.Quantities[i]);
        }
        PlayerInventory.inv.Inventory.Refresh = true;
    }

    public Blueprint CurrentBlueprint
    {
        get
        {
            return _Current;
        }
        set
        {
            if (value == _Current)
                return;
            _Current = value;
            Refresh();
        }
    }

    private Blueprint _Current;

    public void Refresh()
    {
        Clear();
        if (CurrentBlueprint == null)        
            return;
        
        for (int i = 0; i < CurrentBlueprint.Products.Length; i++)
        {
            BlueprintRequirement r = Instantiate(Prefab, Parent);
            r.Item = CurrentBlueprint.Products[i];
            r.Amount = CurrentBlueprint.Quantities[i];
            (r.transform as RectTransform).anchoredPosition = new Vector2(0, -50 * i);
            Spawned.Add(r);
        }
        (Parent.transform as RectTransform).sizeDelta = new Vector2(0, 50 * CurrentBlueprint.Products.Length);
        foreach (BlueprintRequirement r in Spawned)
        {
            r.InInventory = true;
        }
    }

    public void Clear()
    {
        foreach (BlueprintRequirement g in Spawned)
        {
            Destroy(g.gameObject);
        }
        Spawned.Clear();
    }
}
