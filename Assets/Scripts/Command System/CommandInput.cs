using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CommandInput : MonoBehaviour {

    public bool Open;
    public float ClosedX;
    public AnimationCurve Curve;
    public float OpenTime;

    public Text Log;
    public Text Error;
    private InputField input;
    private int index;

    public void Start()
    {
        CommandProcessing.RefreshCommands();
    }

    private Vector2 pos = new Vector2();
    public void Update()
    {
        if (InputManager.InputDown("Console"))
        {
            Open = !Open;
        }



        if (!Open)
        {
            input.DeactivateInputField();
            return;
        }

        if (UnityEngine.Input.GetKeyDown(KeyCode.UpArrow))
        {
            index--;
            if (index < 0)
                index = 0;

            input.text = CommandProcessing.lastCommands[index];
            EventSystem.current.SetSelectedGameObject(input.gameObject, null);
            input.ActivateInputField();
        }
        if (UnityEngine.Input.GetKeyDown(KeyCode.DownArrow))
        {
            index++;
            if (index >= CommandProcessing.lastCommands.Count)
                index = CommandProcessing.lastCommands.Count - 1;

            input.text = CommandProcessing.lastCommands[index];
            EventSystem.current.SetSelectedGameObject(input.gameObject, null);
            input.ActivateInputField();
        }
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
                index = CommandProcessing.lastCommands.Count;            
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
                index = CommandProcessing.lastCommands.Count;
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
