
using UnityEngine;

[CreateAssetMenu(fileName = "RenderData", menuName = "Tiles/Tile Render Data", order = 2)]
public class TileRenderData : ScriptableObject
{
    [SerializeField]
    private Sprite[] Sprites;

    [SerializeField]
    private Color[][] Cache = new Color[16][];

    public Sprite GetSprite(int index)
    {
        return Sprites[index];
    }

    public bool IsCached(int index)
    {
        if (index >= 0 && index < Cache.Length)
            return Cache[index] != null;
        else
            return false;
    }

    public Color[] GetCachedPixels(int index)
    {
        if (IsCached(index))
        {
            return Cache[index];
        }

        return null;
    }

    public void SetCachedPixels(int index, Color[] pixels)
    {
        Cache[index] = pixels;
    }

    public string GetError()
    {
        if(Sprites.Length != 16)
        {
            return "There are " + Sprites.Length + " sprites, but there should be 16";
        }

        int index = 0;
        foreach(Sprite s in Sprites)
        {
            if (s == null)
            {
                return "Sprite at index " + index + " is null!";
            }
            index++;
        }

        return null;
    }
}