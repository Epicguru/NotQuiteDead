using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Item))]
public class TileItem : NetworkBehaviour
{
    public GameTile Tile;
    public TileMapLayer Layer = TileMapLayer.FOREGROUND;

    private Item Item;

    public void Start()
    {
        Item = GetComponent<Item>();
    }

    public void Update()
    {
        if(Item.IsEquipped() && hasAuthority)
        {
            if (InputManager.InputPressed("Shoot")) // TODO - Pressed or down?
            {
                // Place in world and remove from hands.
                Vector3Int unprojected = TiledMap._Instance.Foreground.WorldToCell(InputManager.GetMousePos());

                if (TiledMap._Instance.GetLayer(Layer).HasTile(unprojected))
                    return; // Cannot place

                Player.Local.NetUtils.CmdPlaceTile("Tiles/" + Tile.name, unprojected.x, unprojected.y, Layer);

                // Remove this...
                Player.Local.Holding.CmdDrop(false, true, Player.Local.gameObject, new ItemData());

                // Try to find more items of type.
                InventoryItem i = PlayerInventory.inv.Inventory.GetOfType(Item.Prefab);
                if (i != null)
                {
                    Item.Option_Equip(i, i.ItemPrefab);
                }
            }
        }
    }
}