
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(GameTime))]
public class DayNightCycle : NetworkBehaviour
{
    [HideInInspector]
    public GameTime GT;

    [Tooltip("The ambient light to be applied each day.")]
    public Gradient AmbientLight;

    public void Awake()
    {
        GT = GetComponent<GameTime>();
    }

    public bool IsDay
    {
        get
        {
            if (GT == null)
                return false;

            // Get the hour, in the range 0 to 23 (24 is equal to zero)
            int hour = GT.GetHour();

            // Daytime is 6 to 9 (5 is night time, 10 is night time)
            bool daytime = hour > 5 && hour < 10;

            return daytime;
        }
    }

    public Color GetAmbientLight()
    {
        if (GT == null)
            return Color.white;

        return GetAmbientLight(GT.GetDayProgress());
    }

    public Color GetAmbientLight(float time)
    {
        time = Mathf.Clamp(time, 0f, 1f);

        if (AmbientLight == null)
            return Color.white;

        return AmbientLight.Evaluate(time);
    }
}