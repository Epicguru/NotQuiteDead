using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static class CommandProcessing
{
    public static List<string> lastCommands = new List<string>();
    public static CommandInput input;
    private static List<Command> commands = new List<Command>();

    public static List<Command> GetCommands()
    {
        return commands;
    }

    private static List<Command> tempCommands = new List<Command>();

    public static void RefreshCommands()
    {
        commands.Clear();

        // Default commands
        commands.Add(new HelpCommand());
        commands.Add(new ClearCommand());
        commands.Add(new GiveOneCommand());
        commands.Add(new GiveCommand());
        commands.Add(new HealthCommand());
        commands.Add(new TeleportCommand());
        commands.Add(new PingCommand());
        commands.Add(new PauseCommand());
        commands.Add(new TimescaleCommand());
        commands.Add(new HistoryCommand());
        commands.Add(new InventoryCommands());
        commands.Add(new PoolCommand());
        commands.Add(new ItemsCommand());
    }

    // Returns true to clear the console.
    public static bool Process(string command)
    {
        Error("");

        // Is null or empty
        if (string.IsNullOrEmpty(command.Trim()))
        {
            Error("Input is empty!");
            return false;
        }

        string[] parts = SplitCommand(command);
        if (parts == null)// There was an error!
        {
            return false;
        }

        // Make objects!
        object[] objects = new object[parts.Length];

        // TODO
        int x = 0;
        foreach(string s in parts)
        {
            float f;
            int i2;
            bool b;
            if (float.TryParse(s, out f))
            {
                // Is float, or int.
                // Give float priority.
                objects[x] = f;
            }
            else if (int.TryParse(s, out i2))
            {
                // Is int, but not float.
                objects[x] = i2;

            }
            else if (bool.TryParse(s, out b))
            {
                // Is bool
                objects[x] = b;
            }
            else
            {
                // Is string
                objects[x] = s;
            }

            x++;
        }

        // Find command...
        tempCommands.Clear();
        foreach(Command comm in commands)
        {
            if(comm.Name == parts[0])
            {
                tempCommands.Add(comm);
            }
        }

        object[] args = new object[parts.Length - 1];
        for(int i3 = 1; i3 < objects.Length; i3++)
        {
            args[i3 - 1] = objects[i3];
        }

        if(tempCommands.Count == 0)
        {
            Error("No commands found: '" + parts[0] + "'. Try typing help.");
            return false;
        }


        if(lastCommands.Count == 0 || lastCommands[lastCommands.Count - 1] != command)
        {
            lastCommands.Add(command);
        }

        int i = 0;
        foreach(Command c in tempCommands)
        {
            bool isGood = args.Length == c.parameters.Count  && c.IsValid(args);
            bool isLast = i == tempCommands.Count - 1;

            if (isGood)
            {
                for(int z = 0; z < args.Length; z++)
                {
                    Type t = c.parameters[z];

                    if(t == typeof(int))
                    {
                        // Cast to int
                        args[z] = int.Parse(((Single)(args[z])).ToString()); // That is a float - EDIT: Is a wrapper class of course.
                    }
                }

                Log("Running: " + command);

                // Execute
                string value = c.Execute(args);
                if(value != null)
                {
                    Error("Error in execution. See log.");
                    Log(parts[0] + " error: " + RichText.InColour(value, Color.red));
                    return false;
                }

                tempCommands.Clear();
                return true;
            }

            if (!isGood && isLast)
            {
                if(tempCommands.Count > 1)
                {
                    Error("Args did not match any of the " + tempCommands.Count + " '" + parts[0] + "' commands! See log.");

                    Log(RichText.InColour("Error: No match for args (" + GetArgsAsString(builder, args) + ")", Color.red));
                    Log(RichText.InColour("Try the help command.", Color.red));
                    tempCommands.Clear();

                    return false;
                }
                else
                {
                    // Only one command
                    Error("Wrong arguments for '" + parts[0] + "'. See log.");
                    Log(RichText.InColour("Error: No match for args (" + GetArgsAsString(builder, args) + ")", Color.red));
                    Log(RichText.InColour("Try the help command.", Color.red));
                    tempCommands.Clear();
                    return false;
                }
            }

            i++;
        }

        tempCommands.Clear();

        // Done, successful execution.

        Log("Run: '" + command + "'");

        return false;
    }

    private static StringBuilder builder = new StringBuilder();
    private static List<string> tempParts = new List<string>();
    public static string[] SplitCommand(string command)
    {
        // Returns the split parts, or null of there is an error.
        bool inLiteral = false;
        builder.Length = 0;
        tempParts.Clear();

        const char stringStart = '"';
        const char space = ' ';

        foreach(char c in command)
        {
            if(c == stringStart)
            {
                if (!inLiteral)
                {
                    inLiteral = true;
                    if (!string.IsNullOrEmpty(builder.ToString()))
                    {
                        tempParts.Add(builder.ToString());
                    }
                    // Cut off here, as if there were a space...
                    builder.Length = 0;
                }
                else
                {
                    inLiteral = false;
                    if (!string.IsNullOrEmpty(builder.ToString()))
                    {
                        tempParts.Add(builder.ToString());
                    }
                    builder.Length = 0;
                }
            }
            else if(!inLiteral && c == space)
            {
                // Break, new part
                if (string.IsNullOrEmpty(builder.ToString()))
                    continue;
                tempParts.Add(builder.ToString());
                builder.Length = 0;
            }
            else
            {
                builder.Append(c);
            }
        }

        if(builder.Length > 0)
        {
            if (!string.IsNullOrEmpty(builder.ToString()))
                tempParts.Add(builder.ToString());
        }

        if (inLiteral)
        {
            // Cannot end command on literal!
            Error("Cannot end command on literal! Check the quotes!");
            return null;
        }

        string[] parts = tempParts.ToArray();
        tempParts.Clear();
        return parts;
    }

    private static string GetArgsAsString(StringBuilder s, object[] args)
    {
        s.Length = 0;

        int i = 0;
        foreach (object o in args)
        {
            s.Append(i.GetType().Name + ((i == args.Length - 1) ? "" : ", "));
            i++;
        }

        return s.ToString();
    }

    public static void Log(string line)
    {
        input.Log.text = line + "\n" + input.Log.text;
    }

    public static void ClearLog()
    {
        input.Log.text = "Cleared console.";
    }

    public static void Error(string message)
    {
        input.Error.text = message;
    }
}
