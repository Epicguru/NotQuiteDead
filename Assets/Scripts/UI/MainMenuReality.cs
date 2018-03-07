using UnityEngine;
using UnityEngine.UI;

public class MainMenuReality : MonoBehaviour
{
    [Header("Header")]
    public Text NameText;
    public Image IconImage;

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
        Debug.Log("Clicked on '" + RealityName + "'");
    }
}