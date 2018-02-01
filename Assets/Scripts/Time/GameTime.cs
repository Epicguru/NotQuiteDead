
using UnityEngine.Networking;

public class GameTime : NetworkBehaviour
{
    // Holds the current in-game time of day, month, year etc.
    public static GameTime Instance;

    // In in-game days.
    // Value of 0.00 is midnight day zero.
    // Value of 0.25 is morning day zero.
    // Value of 0.50 is midday day zero.
    // Value of 0.75 is afternoon day zero.
    // Value of 1.00 is midnight day one.
    [SyncVar]
    private float time;

    public void Awake()
    {
        Instance = this;
    }

    public void OnDestroy()
    {
        Instance = null;
    }

    public void Update()
    {
        
    }

    public int GetDay()
    {
        // Easy.
        return (int)time;
    }

    public int GetHour()
    {
        int day = GetDay();

        // A value from 0 to 1.
        float remainder = time - day;

        // Zero means 0.
        // One means 24.
        int hours = 24;

        return (int)(hours * remainder);
    }
}