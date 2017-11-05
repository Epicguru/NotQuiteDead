#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class SimpleTile : GameTile
{
    [SerializeField]
    public Sprite[] m_Sprites;

    public override void GetTileData(Vector3Int location, ITilemap tileMap, ref TileData tileData)
    {
        base.GetTileData(location, tileMap, ref tileData);
        if ((m_Sprites != null) && (m_Sprites.Length > 0))
        {
            long hash = location.x;
            hash = (hash + 0xabcd1234) + (hash << 15);
            hash = (hash + 0x0987efab) ^ (hash >> 11);
            hash ^= location.y;
            hash = (hash + 0x46ac12fd) + (hash << 7);
            hash = (hash + 0xbe9730af) ^ (hash << 11);
            Random.InitState((int)hash);
            tileData.sprite = m_Sprites[(int)(m_Sprites.Length * Random.value)];
        }
    }

#if UNITY_EDITOR
    [MenuItem("Assets/Create/Simple Tile")]
    public static void CreateRandomTile()
    {
        string path = EditorUtility.SaveFilePanelInProject("Save Random Tile", "New Random Tile", "asset", "Save Random Tile", "Assets/Prefabs/Tiles");

        if (path == "")
            return;

        AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<SimpleTile>(), path);
    }
#endif
}

#if UNITY_EDITOR
[CustomEditor(typeof(SimpleTile))]
public class RandomTileEditor : Editor
{
    private SimpleTile tile { get { return (target as SimpleTile); } }
}
#endif