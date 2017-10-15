using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueprintsResults : MonoBehaviour
{
    public Workbench Workbench;

    public void CraftButton()
    {
        // Assume that the button is not pressed when crafting is not available.

        Blueprint b = Workbench.CurrentBlueprint;

        for (int i = 0; i < b.Requirements.Length; i++)
        {
            // TODO MAKE WORK WITH NON_STACKABLES!!!
            PlayerInventory.Remove(b.Requirements[i], b.RequirementQuantities[i], false);
        }

        for (int i = 0; i < b.Products.Length; i++)
        {
            PlayerInventory.Add(b.Products[i], null, b.Quantities[i]);
        }
    }
}
