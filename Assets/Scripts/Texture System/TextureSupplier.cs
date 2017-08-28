using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class TextureSupplier : MonoBehaviour {

    public void Start()
    {
        SpriteAtlasManager.atlasRequested += AtlasRequested;
    }

    private static void AtlasRequested(string tag, System.Action<SpriteAtlas> action)
    {
        bool point = false;
        string name = point ? tag + " Point" : tag;
        Debug.Log("Loading " + "Resources/Atlas/" + name);

        SpriteAtlas loaded = Resources.Load<SpriteAtlas>("Atlas/" + name);

        action(loaded);
    }
}
