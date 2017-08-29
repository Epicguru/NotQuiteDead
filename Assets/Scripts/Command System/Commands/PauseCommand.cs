using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class PauseCommand : Command
{
    public PauseCommand()
    {
        Name = "pause";
    }

    public override string Execute(object[] args)
    {
        if(Time.timeScale != 1)
        {
            Time.timeScale = 1;
        }
        else
        {
            Time.timeScale = 0;
        }

        return null;
    }
}
