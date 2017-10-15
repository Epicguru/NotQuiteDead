﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueprintRequirementDisplay : MonoBehaviour
{
    public BlueprintRequirement Prefab;
    public Transform Parent;
    public List<BlueprintRequirement> Spawned = new List<BlueprintRequirement>();

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
        for (int i = 0; i < CurrentBlueprint.Requirements.Length; i++)
        {
            BlueprintRequirement r = Instantiate(Prefab, Parent);
            r.Item = CurrentBlueprint.Requirements[i];
            r.Amount = CurrentBlueprint.RequirementQuantities[i];
            (r.transform as RectTransform).anchoredPosition = new Vector2(0, -50 * i);
            Spawned.Add(r);
        }
        (Parent.transform as RectTransform).sizeDelta = new Vector2(0, 50 * CurrentBlueprint.Requirements.Length);
        foreach (BlueprintRequirement r in Spawned)
        {
            r.InInventory = PlayerInventory.inv.Inventory.Contains(r.Item, r.Amount);
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
