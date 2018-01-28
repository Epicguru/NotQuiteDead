
using UnityEngine;
using UnityEngine.UI;

public class BuildingItemGhost : MonoBehaviour
{
    public string Prefab;
    public Sprite Icon;

    [Header("Editor")]
    [SerializeField]
    private Image Image;

    public void UpdateVisuals()
    {
        Image.sprite = Icon;
    }
}