
using UnityEngine;

[CreateAssetMenu(fileName = "RenderData", menuName = "Tiles/Tile Render Data", order = 2)]
public class TileRenderData : ScriptableObject
{
    [SerializeField]
    private Sprite[] Sprites;



    public Sprite GetSprite(int index)
    {
        return Sprites[index];
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