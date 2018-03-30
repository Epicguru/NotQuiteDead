using UnityEngine;
using UnityEngine.UI;

public class InventoryItemUI : MonoBehaviour
{
    [Header("References")]
    public Text NameText;
    public Image IconImage;

    [Header("Controls")]
    public string Name;
    public Sprite Icon;
    public int Count;

    [Header("Data")]
    public string Prefab;
    public int Index;

    public InventoryUI InventoryUI
    {
        get
        {
            if(inventory == null)
            {
                inventory = GetComponentInParent<InventoryUI>();
            }
            return inventory;
        }
    }
    private InventoryUI inventory;

    public void Start()
    {
        UpdateVisuals();
    }

    public void UpdateVisuals()
    {
        NameText.text = Name.Trim() + (Count > 1 ? " x" + Count : "");
        IconImage.sprite = Icon;
    }

    public void Clicked()
    {
        Debug.Log("Clicked '{0}' in UI.".Form("Prefab"));

        if(InventoryUI != null)
        {
            if(InventoryUI.Inventory != null)
            {
                // Get the stack through the parent UI which references the real UI.
                ItemStack stack = null;
                try
                {
                    stack = InventoryUI.Inventory.Content[Prefab][Index];
                }
                catch
                {
                    Debug.LogWarning("Exception when trying to get the ItemStack from the UI item. Local data: Prefab - {0}, Count - {1}, Index - {2}. Probably needs to sync up again.".Form(Prefab, Count, Index));
                }

                if (stack != null)
                {
                    InventoryUI.ItemClicked(stack);
                }
            }
        }
    }
}