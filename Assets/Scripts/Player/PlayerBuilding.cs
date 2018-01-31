
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
    [SerializeField]
    private bool menuOpen;

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
        UpdateSelected();

        BuildingUI.Instance.BarOpen = InBuildMode;
        BuildingUI.Instance.MenuOpen = menuOpen;
    }

    private void UpdateModeToggle()
    {
        if (menuOpen)
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
        if (menuOpen)
        {
            if (InputManager.InputDown("Toggle Build Mode", true))
            {
                menuOpen = false;
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
            menuOpen = true;
        }
    }

    private void UpdateSelected()
    {
        // TODO - I would like to use the scrollwheel, but that controls zoom!

        if (BuildingUI.Instance == null)
            return;
        if (!InBuildMode)
            return;

        float delta = Input.mouseScrollDelta.y;

        if(delta != 0f)
        {
            bool up = delta > 0f;

            int selected = BuildingUI.Instance.Bar.SelectedIndex;
            if (up)
            {
                selected++;
            }
            else
            {
                selected--;
            }

            selected = Mathf.Clamp(selected, 0, BuildingUI.Instance.Bar.Items.Count - 1);
            BuildingUI.Instance.Bar.SelectedIndex = selected;
        }
    }
}