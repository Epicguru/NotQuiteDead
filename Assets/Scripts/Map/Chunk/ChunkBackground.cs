using UnityEngine;

public class ChunkBackground : MonoBehaviour
{
    [Header("Controls")]
    public int ChunkSize = 16;
    public Sprite CurrentBG;
    public Sprite BlendedBG;
    public Sprite Mask;

    [Header("References")]
    public Transform Scale;
    public SpriteRenderer Renderer;

    public void Start()
    {
        Apply();
    }

    public void Apply()
    {
        Scale.localScale = Vector3.one * ChunkSize;
        Renderer.material.SetFloat("_Tiling", ChunkSize);
        Renderer.material.SetTexture("_OtherTex", BlendedBG.texture);
        Renderer.material.SetTexture("_MaskTex", Mask.texture);
        Renderer.sprite = CurrentBG;
    }
}