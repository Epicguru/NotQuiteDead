
using UnityEngine;
using UnityEngine.UI;

public class BuildingBarItem : MonoBehaviour
{
    [Header("Data (Code)")]
    public Sprite Icon;
    public string Name;
    public string Prefab;

    [Header("Editor")]
    [SerializeField]
    private Image image;
    [SerializeField]
    private Image background;
    [SerializeField]
    private Color defaultColour;
    [SerializeField]
    private Color selectedColour;

    public void UpdateVisuals(bool selected)
    {
        image.sprite = Icon;        
        UpdateSelected(selected);
    }

    public void UpdateSelected(bool selected)
    {
        background.color = selected ? selectedColour : defaultColour;
    }

    public void SetText(Text text, int count)
    {
        text.text = Name.Trim() + " x" + count;
    }
}