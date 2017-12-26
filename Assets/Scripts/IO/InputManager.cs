using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class InputManager
{

    /*
    * A class that allows for the user to edit key bindings, at runtime.
    * Thanks a bunch unity. Had to make a ridiculously rigid system, didn't you?!?
    * This is a sort of wrapper around unity's class 'Input'.
    */

    public static bool Active = true;

    private static Dictionary<string, KeyCode> keyBindings;
    private static Dictionary<string, KeyCode> defaultKeyBindings;
    private static Vector2 mousePos = new Vector2();

    private static void CreateDefaultBindings()
    {
        defaultKeyBindings = new Dictionary<string, KeyCode>();

        // Add all defaults here.
        // This will never change.
        // If a value is missing from the real key bindings, the default one will be supplied.

        // Player movement.
        defaultKeyBindings.Add("Up", KeyCode.W);
        defaultKeyBindings.Add("Down", KeyCode.S);
        defaultKeyBindings.Add("Right", KeyCode.D);
        defaultKeyBindings.Add("Left", KeyCode.A);
        defaultKeyBindings.Add("Sprint", KeyCode.LeftShift);
        
        // Shooting
        defaultKeyBindings.Add("Shoot", KeyCode.Mouse0);
        defaultKeyBindings.Add("Aim", KeyCode.Mouse1);
        defaultKeyBindings.Add("Reload", KeyCode.R);
        defaultKeyBindings.Add("Firing Mode", KeyCode.X);

        // Inventory
        defaultKeyBindings.Add("Escape", KeyCode.Escape);
        defaultKeyBindings.Add("Inventory", KeyCode.Tab);
        defaultKeyBindings.Add("Return", KeyCode.Escape);
        defaultKeyBindings.Add("Quick Equip", KeyCode.LeftShift);
        defaultKeyBindings.Add("Quick Store", KeyCode.LeftShift);
        defaultKeyBindings.Add("Quick Drop", KeyCode.LeftControl);

        // Control
        defaultKeyBindings.Add("Fullscreen", KeyCode.F11);
        defaultKeyBindings.Add("Key Config", KeyCode.F1);

        // Interaction
        defaultKeyBindings.Add("Pick up", KeyCode.E);
        defaultKeyBindings.Add("Interact", KeyCode.F);

        // Debug
        defaultKeyBindings.Add("Console", KeyCode.KeypadMinus);
        defaultKeyBindings.Add("Debug View", KeyCode.F2);
    }

    public static void MergeKeyBindings()
    {
        // Merges the default and actual key bindings in order to have no missing values.
        // This will not replace current values.

        if (defaultKeyBindings == null)
        {
            Debug.LogError("Default Key Bindings object is null! Cannot merge.");
            return;
        }

        if (keyBindings == null)
        {
            // Such as when we load from file, but the file was not found...
            keyBindings = new Dictionary<string, KeyCode>();
        }

        // Itterate through all values and add any values that are not found.
        foreach (string key in defaultKeyBindings.Keys)
        {
            // Check to see if there is a 'real' value.
            if (keyBindings.ContainsKey(key))
            {
                // Skip becuase we are keeping a real values.
                continue;
            }

            // Put default values.
            // This means that if something is missing in file, it will be saved once the game quits.
            keyBindings.Add(key, defaultKeyBindings[key]);
            Debug.LogWarning("Added missing key binding : '" + key + "' of default value '" + defaultKeyBindings[key] + "'.");
        }
    }

    public static void SetDefaultKeyBindings()
    {
        if(defaultKeyBindings == null)
        {
            Debug.LogError("Default key bindings is null, cannot apply default keys!");
            return;
        }

        keyBindings = new Dictionary<string, KeyCode>();

        foreach(string s in defaultKeyBindings.Keys)
        {
            keyBindings.Add(s, defaultKeyBindings[s]);           
        }

        Debug.Log("Applied all " + keyBindings.Count + " default key bindings.");
    }

    public static void LoadKeyBindings()
    {
        // Creates the default key bindings, which never changes.
        CreateDefaultBindings();

        string path = OutputUtils.InputSaveKeyBindings;
        Debug.Log("Loading key bindings from '" + path + "'...");

        // Load from file, may return null!
        Dictionary<string, KeyCode> x = InputUtils.FileToObject<Dictionary<string, KeyCode>>(path);
        if (x == null)
        {
            Debug.LogWarning("File was not found or parsing error occured. (Pre conversion, file)");
            Debug.Log("Saving default key bindings...");

            // Set the default key bindings and save, because no file exists.
            SetDefaultKeyBindings();
            SaveKeyBindings();

            return;
        }
        else
        {
            keyBindings = x;
        }
        // At this point, key bindings object may be null becuase it was not found on file.
        // Merging the default values will create the object if it is null.
        // When we merge the default values will replace an missing ones.
        if (keyBindings != null)
            Debug.Log("Done! Loaded " + keyBindings.Count + " key bindings!");

        // Merge key bindings.
        MergeKeyBindings();
    }

    public static void SaveKeyBindings()
    {
        string path = OutputUtils.InputSaveKeyBindings;
        Debug.Log("Saving key bindings to '" + path + "'...");
        OutputUtils.ObjectToFile(keyBindings, path); // Will be auto
    }

    public static bool InputPressed(string input, bool bypass = false)
    {
        if (!Active && !bypass)
            return false;
        return Input.GetKey(GetInput(input));
    }

    public static bool InputDown(string input, bool bypass = false)
    {
        if (!Active && !bypass)
            return false;
        return Input.GetKeyDown(GetInput(input));
    }

    public static bool InputUp(string input, bool bypass = false)
    {
        if (!Active && !bypass)
            return false;
        return Input.GetKeyUp(GetInput(input));
    }

    public static void AddInput(string input, KeyCode key)
    {
        keyBindings.Add(input, key);
    }

    public static KeyCode GetInput(string input)
    {
        return keyBindings[input];
    }

    public static void ChangeInput(string input, KeyCode newKey)
    {
        keyBindings[input] = newKey;
    }

    public static void ResetInput(string input)
    {
        keyBindings[input] = defaultKeyBindings[input];
    }

    public static Dictionary<string, KeyCode>.KeyCollection GetInputs()
    {
        return keyBindings.Keys;
    }

    public static bool IsDefault(string input)
    {
        return keyBindings[input] == defaultKeyBindings[input];
    }

    private static List<KeyCode> currentlyPressed = new List<KeyCode>();
    public static KeyCode[] GetAllKeysPressed()
    {
        currentlyPressed.Clear();
        foreach(KeyCode k in Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKey(k))
                currentlyPressed.Add(k);
        }

        return currentlyPressed.ToArray();
    }

    public static KeyCode[] GetAllKeysDown()
    {
        currentlyPressed.Clear();
        foreach (KeyCode k in Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(k))
                currentlyPressed.Add(k);
        }

        return currentlyPressed.ToArray();
    }

    public static KeyCode[] GetAllKeysUp()
    {
        currentlyPressed.Clear();
        foreach (KeyCode k in Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyUp(k))
                currentlyPressed.Add(k);
        }

        return currentlyPressed.ToArray();
    }

    public static Vector2 GetMousePos()
    {
        return mousePos;
    }

    public static void UpdateMousePos()
    {
        Camera c = Camera.main;
        Vector3 x = c.ScreenPointToRay(Input.mousePosition).origin;
        mousePos.Set(x.x, x.y);
    }
}
