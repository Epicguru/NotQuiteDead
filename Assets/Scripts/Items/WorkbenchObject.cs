
using UnityEngine;

public class WorkbenchObject : MonoBehaviour
{
    public Placeable Place;
    public ItemPickup Pick;

    public void Update()
    {
        if (!Place.IsPlaced)
            return;

        if (Pick.MouseOver)
        {
            ActionHUD.DisplayAction("Press " + InputManager.GetInput("Interact") + " to use workbench.");
            if (InputManager.InputDown("Interact"))
            {
                Workbench.Bench.Open = true;
            }
        }
    }
}
