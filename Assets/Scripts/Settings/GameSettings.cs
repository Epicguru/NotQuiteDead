using UnityEngine;

public class GameSettings : MonoBehaviour
{
    public static GameSettings Current;

    // Graphic settings...
    public SettingState SoftChunkBackgrounds = SettingState.ENABLED;

    public void Awake()
    {
        Current = this;
    }

    public void OnDestroy()
    {
        Current = null;
    }
}