
using System;
using UnityEngine;
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
    [SerializeField]
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
        DebugText.Log("Time: " + GetTimeFull());
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

    public string GetPartOfDay()
    {
        int hour = GetHour();

        if(hour >= 0 && hour < 6)
        {
            // 0 to 5
            return "Early Morning";
        }
        if(hour >= 6 && hour < 11)
        {
            // 6 to 10
            return "Morning";
        }
        if(hour >= 11 && hour < 14)
        {
            // 11 to 1
            return "Midday";
        }
        if(hour >= 14 && hour < 19)
        {
            // 2 to 6
            return "Afternoon";
        }
        if(hour >= 19 && hour < 22)
        {
            // 7 to 9
            return "Evening";
        }
        if(hour >= 22 && hour < 24)
        {
            return "Nightime";
        }

        return "Unknown_PoD(" + hour + ")";
    }

    public string GetHourNice()
    {
        int hour = GetHour();
        bool PM = hour >= 12;
        if (PM)
            if(hour > 12)
                hour -= 12;
        return hour.ToString() + (PM ? "PM" : "AM");
    }

    public string GetTime()
    {
        return GetHourNice();
    }

    public string GetTimeFull()
    {
        return "Day " + GetDay() + " " + GetPartOfDay() + " - " + GetTime();
    }
}