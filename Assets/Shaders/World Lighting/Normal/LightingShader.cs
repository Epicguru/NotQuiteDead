
using UnityEngine;

[ExecuteInEditMode]
public class LightingShader : MonoBehaviour
{
    [Header("Shaders")]
    public Material LightMaterial;

    [Header("Render Textures")]
    public RenderTexture LightRT;

    public Camera LightCamera;
    [Range(0.1f, 2f)]
    public float Scale = 1f;

    private int oldWidth = -1;
    private int oldHeight = -1;

    public void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, LightMaterial);
    }

    public void LateUpdate()
    {
        int w = Mathf.RoundToInt(Screen.width * Scale);
        int h = Mathf.RoundToInt(Screen.height * Scale);

        if(LightRT == null)
        {
            CreateRenderTextures(w, h, 24);

            // Apply to cameras...
            LightCamera.targetTexture = LightRT;

            LightMaterial.SetTexture("_LightTexture", LightRT);

            Debug.Log("Created the light render textures to be " + w + ", " + h + " after using a scale of " + Scale + ".");
        }
        else if (w != oldWidth || h != oldHeight)
        {
            // Release old render textures.
            LightRT.Release();

            CreateRenderTextures(w, h, 24);

            LightMaterial.SetTexture("_LightTexture", LightRT);

            // Apply to cameras...
            LightCamera.targetTexture = LightRT;

            oldWidth = w;
            oldHeight = h;

            Debug.Log("Re-Created the light render textures to be " + w + ", " + h + " after using a scale of " + Scale + ".");
        }
    }

    private void CreateRenderTextures(int w, int h, int d)
    {
        LightRT = new RenderTexture(w, h, d);
    }
}