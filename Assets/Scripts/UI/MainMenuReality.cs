using UnityEngine;
using UnityEngine.UI;

public class MainMenuReality : MonoBehaviour
{
    [Header("References")]
    public Text NameText;
    public Image IconImage;
    public MainMenuRealityDetails Details;

    [Header("Runtime")]
    public string RealityName;
    public Sprite Icon;

    public void Start()
    {
        UpdateVisuals();
    }

    public void UpdateVisuals()
    {
        NameText.text = string.IsNullOrWhiteSpace(RealityName) ? "(null)" : RealityName.Trim();
        IconImage.sprite = Icon;
    }

    public void Clicked()
    {
        Debug.Log("Selected '" + RealityName + "'");
        Details.RealityName = this.RealityName;
    }
}