
using UnityEngine;

public class AmbientLight : MonoBehaviour
{
    public static AmbientLight Instance;

    [SerializeField]
    private LightingShader Light;

    public Color OverrideColour = Color.clear;

    public void Awake()
    {
        Instance = this;
    }

    public void OnDestroy()
    {
        Instance = null;
    }

    public void LateUpdate()
    {
        if (Light == null)
            return;

        if(OverrideColour == Color.clear)
        {
            if(GameTime.Instance != null)
                Light.AmbientLight = GameTime.Instance.DayNight.GetAmbientLight();
        }
        else
        {
            Light.AmbientLight = OverrideColour;
        }
    }
}