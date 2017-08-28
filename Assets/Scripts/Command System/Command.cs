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

    public string GetParams(StringBuilder s)
    {
        s.Length = 0;

        int i = 0;
        foreach(Type t in parameters)
        {
            s.Append(t.Name + ((i == parameters.Count - 1) ? "" : ", "));
            i++;
        }

        return s.ToString();
    }

    public virtual bool IsValid(object[] args)
    {
        for(int x = 0; x < args.Length; x++)
        {
            object y = args[x];

            // Special case:
            // Some (or all) values may be floats when ints are expected.
            // If we expect an int, try to cast to int. If it fails, return false.
            // If we expect a float, pass.

            if(y.GetType() == typeof(float))
            {
                // Handling
                if(parameters[x] == typeof(int))
                {
                    int i;
                    bool worked = int.TryParse(y.ToString(), out i);

                    if (worked)
                    {
                        // We can cast to the int we requested. Good!
                        continue;
                    }
                    else
                    {
                        // We cannot cast from this potential int. Bad!
                        return false;
                    }
                }
            }

            if (parameters[x] != y.GetType())
                return false;
        }

        return true;
    }
}
