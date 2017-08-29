using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
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

        try
        {
            if (CommandProcessing.Process(text))
            {
                input.text = "";                
            }
        }catch(Exception e)
        {
            CommandProcessing.Log(RichText.InColour("Exception : " + e.Message, Color.red));
        }

        EventSystem.current.SetSelectedGameObject(input.gameObject, null);
        input.ActivateInputField();
    }

    public void ButtonDown()
    {
        try
        {
            if (CommandProcessing.Process(input.text))
            {
                input.text = "";
            }
        }
        catch (Exception e)
        {
            CommandProcessing.Log(RichText.InColour("Exception : " + e.Message, Color.red));
        }

        EventSystem.current.SetSelectedGameObject(input.gameObject, null);
        input.ActivateInputField();
    }
}
