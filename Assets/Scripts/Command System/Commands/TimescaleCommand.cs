using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class TimescaleCommand : Command
{
    public TimescaleCommand()
    {
        Name = "timescale";
        parameters.Add(typeof(float));
    }

    public override string Execute(object[] args)
    {
        float scale = (float)args[0];

        if(scale < 0)
        {
            return "Time scale cannot be smaller than zero!";
        }

        Time.timeScale = scale;

        return null;
    }
}
