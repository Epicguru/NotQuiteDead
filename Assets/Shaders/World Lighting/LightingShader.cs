
using UnityEngine;

[ExecuteInEditMode]
public class LightingShader : MonoBehaviour
{
    public Material Material;
    public RenderTexture RT;
    public Camera LightCamera;
    [Range(0.1f, 2f)]
    public float Scale = 1f;

    private int oldWidth = -1;
    private int oldHeight = -1;

    public void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, Material);
    }

    public void LateUpdate()
    {
        int w = Mathf.RoundToInt(Screen.width * Scale);
        int h = Mathf.RoundToInt(Screen.height * Scale);

        if(RT == null)
        {
            CreateRT(w, h, 24);
            LightCamera.targetTexture = RT;
            Material.SetTexture("LightTexture", RT);
            Debug.Log("Created the light render texture to be " + w + ", " + h + " after using a scale of " + Scale + ".");
        }
        else if (w != oldWidth || h != oldHeight)
        {
            RT.Release(); // So long...
            CreateRT(w, h, 24);
            Material.SetTexture("LightTexture", RT);
            LightCamera.targetTexture = RT;

            oldWidth = w;
            oldHeight = h;

            Debug.Log("Re-Created the light render texture to be " + w + ", " + h + " after using a scale of " + Scale + ".");
        }
    }

    private void CreateRT(int w, int h, int d)
    {
        RT = new RenderTexture(w, h, d);
    }
}