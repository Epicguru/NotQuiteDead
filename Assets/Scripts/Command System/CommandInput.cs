using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CommandInput : MonoBehaviour {

    public Text Log;
    public Text Error;
    private InputField input;

    public void Start()
    {
        CommandProcessing.RefreshCommands();
    }

	public void Input()
    {
        if (input == null)
            input = GetComponentInChildren<InputField>();

        if (CommandProcessing.input != this)
            CommandProcessing.input = this;

        string text = input.text;

        if (!UnityEngine.Input.GetKeyDown(KeyCode.Return))
        {
            return;
        }

        CommandProcessing.Process(text);
    }

    public void ButtonDown()
    {
        CommandProcessing.Process(input.text);
    }
}
