using UnityEngine;
using UnityEngine.UI;

public class BuildingMenuItem : MonoBehaviour
{
    public Color BGColour;
    public string Name;
    public int Count;
    public Sprite Icon;

    [SerializeField]
    private Text text;
    [SerializeField]
    private Image icon;
    [SerializeField]
    private Image BG;

    public void UpdateVisuals()
    {
        text.text = Name.Trim() + " x" + Count;
        icon.sprite = Icon;
        BG.color = BGColour;
    }
}