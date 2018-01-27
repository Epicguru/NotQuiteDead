using UnityEngine;
using UnityEngine.UI;

public class BuildingMenuItem : MonoBehaviour
{
    public Color BGColour;
    public string Name;
    public int Count;
    public Sprite Icon;

    private Text text;
    private Image icon;
    private Image BG;

    public void UpdateVisuals()
    {
        text.text = Name.Trim() + " x" + Count;
        icon.sprite = Icon;
        BG.color = BGColour;
    }
}