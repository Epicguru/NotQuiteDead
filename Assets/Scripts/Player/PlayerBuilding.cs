
using UnityEngine.Networking;
using UnityEngine;

public class PlayerBuilding : NetworkBehaviour
{
    [Header("Controls")]
    public bool InBuildMode;
    public float MaxBuildRange = 15f;
    public float MaxInteractRange = 10f;

    [Header("Menu Opening")]
    public float HoldTime = 0.7f;

    [Header("Preview")]
    public GameObject PreviewPrefab;
    private TilePreview Preview;

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
        UpdatePreview();
        UpdatePlacing();

        BuildingUI.Instance.BarOpen = InBuildMode;
        BuildingUI.Instance.MenuOpen = menuOpen;
    }

    private void UpdatePreview()
    {
        if(Preview == null)
        {
            Preview = Instantiate(PreviewPrefab).GetComponent<TilePreview>();
        }

        bool placementMode = InBuildMode && !menuOpen;

        Preview.gameObject.SetActive(placementMode);
        if (!placementMode)
        {
            return;
        }

        int x = (int)InputManager.GetMousePos().x;
        int y = (int)InputManager.GetMousePos().y;

        string error = CanPlace(x, y);
        bool canPlace = error == null;

        Preview.CanPlace = canPlace;

        if (!canPlace && InputManager.InputDown("Shoot"))
        {
            ErrorMessageUI.Instance.DisplayMessage = "Cannot Place:\n" + error;
        }

        Preview.transform.position = new Vector3(x, y, 0f);
    }

    public string CanPlace(int x, int y)
    {
        bool inBounds = World.Instance.TileMap.InBounds(x, y);
        if (!inBounds)
        {
            return "Out of world bounds!";
        }

        bool placementMode = InBuildMode && !menuOpen;
        if (!placementMode)
        {
            return "Not in placement mode!";
        }

        BuildingItem item = GetSelectedItem();
        bool hasSelected = item != null;
        if (!hasSelected)
        {
            return "No buildable selected! Hold [" + InputManager.GetInput("Toggle Build Mode") + "] to open menu.";
        }

        if(item.Type == BuildingItemType.TILE)
        {
            BaseTile oldTile = World.Instance.TileMap.GetLayer(item.GetTile().Layer).GetTile(x, y);

            if (oldTile != null)
            {
                return "Space already occupied!";
            }

            bool canPlaceTile = World.Instance.TileMap.GetLayer(item.GetTile().Layer).CanPlaceTile(x, y);
            if (!canPlaceTile)
            {
                return "Space already occupied!";
            }
        }
        else if(item.Type == BuildingItemType.FURNITURE)
        {
            bool canPlaceFurniture = !World.Instance.Furniture.IsFurnitureAt(x, y) && World.Instance.TileMap.GetLayer(item.GetFurniture().Layer).GetTile(x, y) == null;
            if (!canPlaceFurniture)
            {
                return "Space already occupied!";
            }
        }

        return null;
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

        int selected = BuildingUI.Instance.Bar.SelectedIndex;
        if(delta != 0f)
        {
            bool up = delta > 0f;

            if (up)
            {
                selected++;
            }
            else
            {
                selected--;
            }

        }
        selected = Mathf.Clamp(selected, 0, BuildingUI.Instance.Bar.Items.Count - 1);
        BuildingUI.Instance.Bar.SelectedIndex = selected;
    }

    public BuildingItem GetSelectedItem()
    {
        int index = BuildingUI.Instance.Bar.SelectedIndex;
        if (index < 0 || index >= BuildingUI.Instance.Bar.Items.Count)
            return null;

        BuildingItem item = BuildingUI.Instance.Bar.Items[index];
        return item;
    }

    private void UpdatePlacing()
    {
        if (!InBuildMode)
            return;

        if (!InputManager.InputPressed("Shoot"))
            return;

        int x = (int)InputManager.GetMousePos().x;
        int y = (int)InputManager.GetMousePos().y;
        if (CanPlace(x, y) != null)
            return;

        if (Player.Local == null)
            return;

        if (BuildingUI.Instance == null)
            return;

        BuildingItem item = GetSelectedItem();
        if(item != null)
        {
            // Place this item where the mouse is, and remove one from the inventory.

            string layer = "Foreground";

            switch (item.Type)
            {
                case BuildingItemType.FURNITURE:

                    Furniture f = item.GetFurniture();
                    layer = f.Layer;

                    // Place furniture.
                    bool worked = World.Instance.Furniture.PlaceFurniture(f.Prefab, x, y);

                    // Remove one from inventory.
                    if(worked)
                        Player.Local.BuildingInventory.RemoveItems(f.Prefab, 1);

                    break;

                case BuildingItemType.TILE:

                    BaseTile tile = item.GetTile();
                    layer = tile.Layer;

                    BaseTile oldTile = World.Instance.TileMap.GetLayer(layer).GetTile(x, y);

                    if (oldTile != null && oldTile.Prefab == tile.Prefab)
                    {
                        return;
                    }

                    // Place tile.
                    worked = World.Instance.TileMap.GetLayer(layer).SetTile(tile, x, y);

                    // Remove one from inventory.
                    if(worked)
                        Player.Local.BuildingInventory.RemoveItems(tile.Prefab, 1);

                    break;
            }
        }
    }
}