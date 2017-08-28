using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[System.Serializable]
public abstract class Command
{
    public string Name;
    public List<Type> parameters = new List<Type>();

    /// <summary>
    /// Execute the command. Should return null if successful, or an error message if failed.
    /// </summary>
    public abstract string Execute(object[] args);

    public virtual bool IsValid(string Name, object[] args)
    {
        for(int x = 0; x < args.Length; x++)
        {
            object y = args[x];

            if (parameters[x] != y.GetType())
                return false;
        }

        return true;
    }
}
