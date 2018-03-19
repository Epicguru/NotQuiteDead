
using UnityEngine;
using UnityEngine.Networking;

public class WorkbenchFurniture : NetworkBehaviour
{
    public Furniture Furniture;

    public void Update()
    {
        Vector2 mousePos = InputManager.GetMousePos();
        int mouseX = (int)mousePos.x;
        int mouseY = (int)mousePos.y;
        bool mouseInside = mouseX == Furniture.X && mouseY == Furniture.Y;

        if (mouseInside)
        {
            string key = InputManager.GetInput("Interact").ToString();
            ActionHUD.DisplayAction("Blueprint_OpenPrompt".Translate(key));
            if (InputManager.InputDown("Interact"))
            {
                Workbench.Bench.Open = true;
            }
        }
    }
}