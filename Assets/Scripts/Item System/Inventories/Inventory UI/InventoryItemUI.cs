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
        Debug.Log("Clicked '{0}'".Form("Prefab"));
    }
}