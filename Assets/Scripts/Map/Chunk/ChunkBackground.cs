using UnityEngine;
using System.Linq;
using UnityEngine.Rendering;

public class ChunkBackground : MonoBehaviour
{
    public static ChunkBackground[] Surroundings;

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
            }
        }
    }
    [SerializeField]
    private Background _BG;

    public Texture SourceTexture;
    public RenderTexture Target;
    public Material Shader;
    public Material DefaultShader;

    [Header("References")]
    public Transform BackgroundTransform;
    public MeshRenderer Renderer;
    public Chunk Chunk;

    public Texture[] Masks;

    public void Start()
    {
        CreateTexture2D();
        UpdatePositioning();
    }

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
        var bgs = GetSurroundings();
        var sorted = SortByOrder(bgs);

        Regenerate(sorted);
    }

    public void Regenerate(Background[] bgs)
    {
        if (Shader == null)
            return;
        if (bgs == null)
            return;

        if (BG == null)
            return;

        SourceTexture = BG.Sprite == null ? null : BG.Sprite.texture;
        if (SourceTexture == null)
            return;
        
        if(bgs.Length == 0)
        {
            Graphics.Blit(SourceTexture, Target, DefaultShader);
        }
        else
        {
            for (int i = 0; i < bgs.Length; i++)
            {
                Debug.Log("Chunk ({0}): Index {1}, Value {2}, Mask Index {3}".Form(name, i, bgs[i] == null ? "null" : bgs[i].Name, bgs[i] == null ? -1 : bgs[i].Temp_MaskIndex));
                if (bgs[i] == null)
                {
                    // After the first null value, all are null...
                    // No need to even loop more.
                    break;
                }

                // Ensure that is has a greater order than this background, otherwise don't even draw it.
                // If it is the same order, of course don't bother.
                int thisOrder = BG.Order;
                int otherOrder = bgs[i].Order;
                if (thisOrder >= otherOrder)
                    continue;

                Texture mask = Masks[bgs[i].Temp_MaskIndex];
                Texture other = bgs[i].Sprite == null ? null : bgs[i].Sprite.texture;

                if (other == null || mask == null)
                    continue;

                Shader.SetTexture("_MaskTex", mask);
                Shader.SetTexture("_OtherTex", other);
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

    public ChunkBackground[] GetSurroundings()
    {
        // Starts top left, goes clockwise.

        if(Surroundings == null)
        {
            Surroundings = new ChunkBackground[8];
        }

        TileLayer layer = Chunk.Layer;
        int x = Chunk.X;
        int y = Chunk.Y;
        int i = -1;

        // Top left.
        x -= 1;
        y += 1;
        i++;
        if (layer.IsChunkLoaded(x, y))
        {
            Chunk c = layer.GetChunkFromChunkCoords(x, y);
            Surroundings[i] = c.Background;
        }
        else
        {
            Surroundings[i] = null;
        }

        // Top center.
        x += 1;
        y += 0;
        i++;
        if (layer.IsChunkLoaded(x, y))
        {
            Chunk c = layer.GetChunkFromChunkCoords(x, y);
            Surroundings[i] = c.Background;
        }
        else
        {
            Surroundings[i] = null;
        }

        // Top right.
        x += 1;
        y = 0;
        i++;
        if (layer.IsChunkLoaded(x, y))
        {
            Chunk c = layer.GetChunkFromChunkCoords(x, y);
            Surroundings[i] = c.Background;
        }
        else
        {
            Surroundings[i] = null;
        }

        // Right center.
        x += 0;
        y -= 1;
        i++;
        if (layer.IsChunkLoaded(x, y))
        {
            Chunk c = layer.GetChunkFromChunkCoords(x, y);
            Surroundings[i] = c.Background;
        }
        else
        {
            Surroundings[i] = null;
        }

        // Right bottom.
        x += 0;
        y -= 1;
        i++;
        if (layer.IsChunkLoaded(x, y))
        {
            Chunk c = layer.GetChunkFromChunkCoords(x, y);
            Surroundings[i] = c.Background;
        }
        else
        {
            Surroundings[i] = null;
        }

        // Bottom center.
        x -= 1;
        y += 0;
        i++;
        if (layer.IsChunkLoaded(x, y))
        {
            Chunk c = layer.GetChunkFromChunkCoords(x, y);
            Surroundings[i] = c.Background;
        }
        else
        {
            Surroundings[i] = null;
        }

        // Left bottom.
        x -= 1;
        y += 0;
        i++;
        if (layer.IsChunkLoaded(x, y))
        {
            Chunk c = layer.GetChunkFromChunkCoords(x, y);
            Surroundings[i] = c.Background;
        }
        else
        {
            Surroundings[i] = null;
        }

        // Left center.
        x += 0;
        y += 1;
        i++;
        if (layer.IsChunkLoaded(x, y))
        {
            Chunk c = layer.GetChunkFromChunkCoords(x, y);
            Surroundings[i] = c.Background;
        }
        else
        {
            Surroundings[i] = null;
        }

        return Surroundings;
    }

    public ChunkBackground[] SortByOrder(ChunkBackground[] bgs)
    {
        if (bgs == null)
            return null;

        var sorted = from x in bgs orderby (x == null ? int.MaxValue : x.BG == null ? int.MaxValue : x.BG.Order) ascending select x;
        return sorted.ToArray();
    }

    public int GetMaskIndex(ChunkBackground bg)
    {
        // Assume that it is 'touching' this chunk, because optimisation.


    }
}