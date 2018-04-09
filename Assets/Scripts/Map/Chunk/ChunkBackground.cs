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
            if (_BG != value)
            {
                _BG = value;
            }
        }
    }
    [SerializeField]
    private Background _BG;

    public float LerpSpeed;
    public Vector2 LerpAngle;
    public Vector2 LerpScale;
    public FilterMode FilterMode;

    public Texture SourceTexture;
    public RenderTexture Target;
    public Material Shader;
    public Material DefaultShader;

    [Header("References")]
    public Transform BackgroundTransform;
    public MeshRenderer Renderer;
    public Chunk Chunk;

    public Texture2D Texture;

    public Texture[] Masks;

    public void DoneLoading()
    {
        SetFinalPosition();
        UpdateBGAndSurroundings();
    }

    public void UpdateBGAndSurroundings()
    {
        // Updates the current background drawing and state, and also tells all surrounding chunks to regenerate their own backgrounds.
        ChunkRegenerator.Instance.Regenerate(this);
        foreach (var item in GetSurroundings())
        {
            if(item != null)
            {
                ChunkRegenerator.Instance.Regenerate(item);
            }
        }
    }

    public void SetFinalPosition()
    {
        const float Scale = 1f;
        BackgroundTransform.localPosition = new Vector3(ChunkSize * 0.5f, ChunkSize * 0.5f, 1);
        BackgroundTransform.localScale = new Vector3(ChunkSize * Scale, -ChunkSize * Scale, 1);
    }

    public void CreateTexture2D()
    {
        if (Texture != null)
            return;

        Texture = new Texture2D(320, 320, TextureFormat.RGBA32, false);
        Texture.filterMode = FilterMode;
        Texture.wrapMode = TextureWrapMode.Clamp;
        Renderer.material.mainTexture = Texture;
    }

    public void Regenerate()
    {
        var bgs = GetSurroundings();
        var sorted = SortByOrder(bgs);

        Regenerate(sorted);
    }

    public void Regenerate(ChunkBackground[] bgs)
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

        CreateTexture2D();

        bool doneAnything = false;
        bool first = true;
        for (int i = 0; i < bgs.Length; i++)
        {
            if (bgs[i] == null)
            {
                // After the first null value, all are null...
                // No need to even loop more.
                //break;
                continue;
            }
            if (bgs[i].BG == null)
            {
                // Just ignore it.
                continue;
            }

            // Ensure that is has a greater order than this background, otherwise don't even draw it.
            // If it is the same order, of course don't bother.
            int thisOrder = BG.Order;
            int otherOrder = bgs[i].BG.Order;
            if (thisOrder >= otherOrder)
            {
                continue;
            }

            int maskIndex = GetMaskIndex(bgs[i]);
            Texture mask = Masks[maskIndex];
            Texture other = bgs[i].BG.Sprite == null ? null : bgs[i].BG.Sprite.texture;

            if (other == null || mask == null)
            {
                continue;
            }

            Shader.SetTexture("_MaskTex", mask);
            Shader.SetTexture("_OtherTex", other);
            Texture source;
            if (first)
            {
                source = SourceTexture;
                first = false;
            }
            else
            {
                source = Target;
            }
            Graphics.Blit(source, Target, Shader);
            doneAnything = true;
        }

        if (!doneAnything)
        {
            Graphics.Blit(SourceTexture, Target, DefaultShader);
        }

        var t2D = Renderer.material.mainTexture as Texture2D;

        RenderTexture old = RenderTexture.active;
        RenderTexture.active = Target;

        // Graphics.CopyTexture(Target, t2D); // Does not apply instantly due to GPU side data only.
        t2D.ReadPixels(new Rect(0, 0, Target.width, Target.height), 0, 0);
        t2D.Apply();

        RenderTexture.active = old;
    }

    public ChunkBackground[] GetSurroundings()
    {
        // Starts top left, goes clockwise.

        if (Surroundings == null)
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
        // Index starts at top left and moves clockwise.

        if (bg == null)
            return -1;

        int diffX = bg.Chunk.X - Chunk.X;
        int diffY = bg.Chunk.Y - Chunk.Y;

        if (diffX == -1)
        {
            if (diffY == 1)
            {
                // Top left.
                return 0;
            }
            if (diffY == 0)
            {
                // Left center.
                return 7;
            }
            if (diffY == -1)
            {
                // Bottom left.
                return 6;
            }
        }
        else if (diffX == 0)
        {
            if (diffY == 1)
            {
                // Top center.
                return 1;
            }
            if (diffY == 0)
            {
                // Dead center, this chunk is me!
                // Lol. Not valid.
                Debug.LogError("Tried to get index for centered chunk!");
                return -1;
            }
            if (diffY == -1)
            {
                // Bottom center.
                return 5;
            }
        }
        else if (diffX == 1)
        {
            if (diffY == 1)
            {
                // Top right.
                return 2;
            }
            if (diffY == 0)
            {
                // Right center.
                return 3;
            }
            if (diffY == -1)
            {
                // Bottom right.
                return 4;
            }
        }
        else
        {
            // No touching...
            Debug.LogError("Error exit point A: DiffX is {0}, DiffY is {1}. Chunk BG is {2}".Form(diffX, diffY, bg.BG));
            return -1;
        }

        // If the code reaches here, it is not touching...
        Debug.LogError("Error exit point B: DiffX is {0}, DiffY is {1}. Chunk BG is {2}".Form(diffX, diffY, bg.BG));
        return -1;
    }

    public void OnDestroy()
    {
        Destroy(Texture);
    }
}