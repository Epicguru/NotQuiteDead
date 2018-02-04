
using UnityEngine;

public class AmbientLight : MonoBehaviour
{
    public static AmbientLight Instance;

    public Color OverrideColour = Color.clear;

    [SerializeField]
    private Camera Camera;

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
        if (Camera == null)
            return;

        if(OverrideColour == Color.clear)
        {
            if(GameTime.Instance != null)
                Camera.backgroundColor = GameTime.Instance.DayNight.GetAmbientLight();
        }
        else
        {
            Camera.backgroundColor = OverrideColour;
        }
    }
}