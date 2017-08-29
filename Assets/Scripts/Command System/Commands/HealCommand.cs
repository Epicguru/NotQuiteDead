using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class HealthCommand : Command
{
    public HealthCommand()
    {
        Name = "health";
        parameters.Add(typeof(float));
    }

    public override string Execute(object[] args)
    {
        float health = (float)args[0];

        if(health > 0)
        {
            Player.Local.Health.CmdHeal(health);
        }
        else
        {
            Player.Local.Health.CmdDamage(health, "Console", false);
        }

        return null;
    }
}
