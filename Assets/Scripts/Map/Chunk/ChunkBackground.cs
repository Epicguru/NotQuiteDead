using UnityEngine;
using UnityEngine.Rendering;

public class ChunkBackground : MonoBehaviour
{
    [Header("Controls")]
    public float ChunkSize = 16;
    public Background BG
    {
        get
        {
            return _BG;
        }
        set
        {
            if(_BG != value)
            {
                _BG = value;
                UpdateVisuals();
            }
        }
    }
    [SerializeField]
    private Background _BG;

    public Sprite HaloSprite;
    public bool DrawHalo = true;

    [Header("References")]
    public Transform Graphics;
    public Transform HaloScale;
    public SpriteRenderer HaloRenderer;
    public SpriteRenderer MainRenderer;
    public SortingGroup Group;

    public void UpdateVisuals()
    {
        Graphics.localPosition = new Vector3(0, 0, 0);
        Group.transform.localPosition = new Vector3(ChunkSize * 0.5f, ChunkSize * 0.5f, 0f);

        if (DrawHalo)
        {
            HaloScale.localScale = Vector3.one * ChunkSize * 3f;
            HaloRenderer.sprite = HaloSprite;
            HaloRenderer.material.SetTexture("_ChunkTex", BG != null ? BG.Sprite == null ? null : BG.Sprite.texture : null);
            HaloRenderer.material.SetFloat("_Tiling", ChunkSize * 3f);
            HaloRenderer.material.SetFloat("_Edge", BG == null ? 1f : BG.EdgeShortness);
            HaloScale.gameObject.SetActive(true);
        }
        else
        {
            HaloScale.gameObject.SetActive(false);
        }

        MainRenderer.sprite = BG == null ? null : BG.Sprite;
        MainRenderer.size = new Vector2(ChunkSize, ChunkSize);

        Group.sortingOrder = BG == null ? 0 : BG.Order;
    }
}