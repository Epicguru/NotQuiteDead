using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static class CommandProcessing
{
    public static List<string> lastCommands = new List<string>();
    public static CommandInput input;

    // Returns true to clear the console.
    public static bool Process(string command)
    {
        Error("");

        // Is null or empty
        if (string.IsNullOrEmpty(command))
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

        // TODO
        foreach(string s in parts)
        {
            Debug.Log(s);
        }

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

    public static void Log(string line)
    {
        input.Log.text = line + "\n" + input.Log.text;
    }

    public static void Error(string message)
    {
        input.Error.text = message;
    }
}
