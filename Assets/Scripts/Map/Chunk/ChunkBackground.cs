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
                UpdatePositioning();
            }
        }
    }
    [SerializeField]
    private Background _BG;

    public Texture SourceTexture;
    public Texture[] MaskTextures;
    public Texture[] OtherTextures;
    public RenderTexture Target;
    public Material Shader;
    public Material DefaultShader;

    [Header("References")]
    public Transform BackgroundTransform;
    public MeshRenderer Renderer;

    public void UpdatePositioning()
    {
        BackgroundTransform.localPosition = new Vector3(ChunkSize * 0.5f, ChunkSize * 0.5f, 0);
        BackgroundTransform.localScale = new Vector3(ChunkSize, ChunkSize, 1);
    }

    public void CreateTexture2D()
    {
        var tex = new Texture2D(320, 320);
        Renderer.material.mainTexture = tex;
    }

    public void Regenerate()
    {
        if (Shader == null)
            return;
        
        if(MaskTextures.Length == 0)
        {
            Graphics.Blit(SourceTexture, Target, DefaultShader);
        }
        else
        {
            for (int i = 0; i < MaskTextures.Length; i++)
            {
                Shader.SetTexture("_MaskTex", MaskTextures[i]);
                Shader.SetTexture("_OtherTex", OtherTextures[i]);
                var source = SourceTexture;
                if (i != 0)
                {
                    source = Target;
                }
                Graphics.Blit(source, Target, Shader);
            }
        }

        var t2D = Renderer.material.mainTexture as Texture2D;

        RenderTexture old = RenderTexture.active;
        RenderTexture.active = Target;

        t2D.ReadPixels(new Rect(0, 0, Target.width, Target.height), 0, 0);
        t2D.Apply();

        RenderTexture.active = old;
    }
}