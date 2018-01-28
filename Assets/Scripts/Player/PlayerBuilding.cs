
using UnityEngine.Networking;
using UnityEngine;

public class PlayerBuilding : NetworkBehaviour
{
    [Header("Controls")]
    public bool InBuildMode;

    [Header("Menu Opening")]
    public float HoldTime = 0.7f;

    [Header("Data")]
    [ReadOnly]
    public bool MenuOpen;

    private float timer;
    private bool clicked;
    private bool justClosed;

    public void Update()
    {
        if (!isLocalPlayer)
            return;

        UpdateMenuOpening();
        UpdateModeToggle();    
        UpdateMenuClosing();

        BuildingUI.Instance.BarOpen = InBuildMode;
        BuildingUI.Instance.MenuOpen = MenuOpen;
    }

    private void UpdateModeToggle()
    {
        if (MenuOpen)
        {
            // Controls are different when within menu.
            return;
        }

        if (InBuildMode)
        {
            if(InputManager.InputUp("Toggle Build Mode"))
            {
                if (clicked)
                {
                    clicked = false;
                    justClosed = false;
                    return;
                }
                if (justClosed)
                {
                    justClosed = false;
                    clicked = false;
                    return;
                }
                InBuildMode = false;
            }
        }
        else
        {
            if (InputManager.InputDown("Toggle Build Mode"))
            {
                InBuildMode = true;
                clicked = true;
            }
        }
    }

    private void UpdateMenuClosing()
    {
        if (MenuOpen)
        {
            if (InputManager.InputDown("Toggle Build Mode", true))
            {
                MenuOpen = false;
                justClosed = true;
            }
        }
    }

    private void UpdateMenuOpening()
    {
        if (UI.AnyOpen)
            return;

        if(InputManager.InputPressed("Toggle Build Mode"))
        {
            // Reset timer.
            timer += Time.unscaledDeltaTime;
        }
        else
        {
            timer = 0;
        }

        if(timer > HoldTime)
        {
            timer = 0;
            MenuOpen = true;
        }
    }
}