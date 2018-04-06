// Used to generate Texture Array asset
// Menu button is available in GameObject > Create Texture Array
// See CHANGEME in the file

// Epicguru here: Many thanks to https://github.com/Spaxe for this simple script!

using UnityEngine;
using UnityEditor;

public class TextureArrayCreator : MonoBehaviour
{
    [MenuItem("Assets/Create Texture Array")]
    static void Create()
    {
        // CHANGEME: Filepath must be under "Resources" and named appropriately. Extension is ignored.
        string filePattern = "Chunk Masks/Mask_{0}";

        // CHANGEME: Number of textures you want to add in the array
        const int slices = 8;
        const int size = 64;

        // CHANGEME: TextureFormat.RGB24 is good for PNG files with no alpha channels. Use TextureFormat.RGB32 with alpha.
        // See Texture2DArray in unity scripting API.
        Texture2DArray textureArray = new Texture2DArray(size, size, slices, TextureFormat.RGB24, false);

        // CHANGEME: If your files start at 001, use i = 1. Otherwise change to what you got.
        for (int i = 0; i < slices; i++)
        {
            string filename = string.Format(filePattern, i);
            Debug.Log("Loading " + filename);
            Texture2D tex = (Texture2D)Resources.Load(filename);

            if(tex == null)
            {
                Debug.LogError("Failed to load {0} from resources, there are {0} slices, the resolution is {1}.".Form(filename, slices, size));
                continue;
            }

            textureArray.SetPixels(tex.GetPixels(), i);
        }
        textureArray.Apply();

        // CHANGEME: Path where you want to save the texture array. It must end in .asset extension for Unity to recognise it.
        string path = "Assets/Chunk Masks.asset";
        AssetDatabase.CreateAsset(textureArray, path);
        Debug.Log("Saved asset to " + path);
    }
}

// After this, you will have a Texture Array asset which you can assign to the shader's Tex attribute!