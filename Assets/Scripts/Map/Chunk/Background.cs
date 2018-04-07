using UnityEngine;

[CreateAssetMenu(fileName = "Chunk Background", menuName = "Tiles/Chunk Background", order = 5)]
public class Background : ScriptableObject
{
    [Header("Data")]
    public string Prefab;
    public string Name;

    [Header("Visuals")]
    public Sprite Sprite;
    [Range(0f, 4f)]
    public float EdgeLength = 4f;

    [Header("Info")]
    [ReadOnly]
    public int Order;

    public virtual float EdgeShortness
    {
        get
        {
            return 5f - (EdgeLength);
        }
    }

    public override string ToString()
    {
        return "{0} ({1})".Form(Prefab, Name);
    }
}