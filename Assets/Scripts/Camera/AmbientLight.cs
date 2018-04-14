
using UnityEngine;

public class AmbientLight : MonoBehaviour
{
    public static AmbientLight Instance;

    [SerializeField]
    private Camera Camera;

    public Color OverrideColour = Color.clear;

    public void Awake()
    {
        Instance = this;
    }

    public void OnDestroy()
    {
        Instance = null;
    }

    public Color GetCurrentColour()
    {
        if (OverrideColour == Color.clear)
        {
            if (GameTime.Instance != null)
                return GameTime.Instance.DayNight.GetAmbientLight();
            else
                return Color.white;
        }
        else
        {
            return OverrideColour;
        }
    }

    public void LateUpdate()
    {
        if (Camera == null)
            return;

        Camera.backgroundColor = GetCurrentColour();
    }
}