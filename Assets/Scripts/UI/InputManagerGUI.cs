using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputManagerGUI : MonoBehaviour
{

    /*
    * A class that represets graphically the InputManager class and allows it to be edited at runtime.
    */
    public Color ColourA, ColourB;

    public static InputManagerGUI Instance;

    public Transform Parent;
    public GameObject Container;
    public GameObject InputScreen;

    private GameObject[] inputs;

    public InputManagerGUI()
    {
        Instance = this;
    }

    public void Start()
    {
        Instance = this;
    }

    public void PlaceAllObjects()
    {
        inputs = new GameObject[InputManager.GetInputs().Count];

        int y = 0;
        int i = 0;
        foreach(string s in InputManager.GetInputs())
        {
            string name = s;
            GameObject instance = Instantiate(Container.gameObject, Parent);
            RectTransform t = instance.GetComponent<RectTransform>();
            t.localPosition = new Vector2(0, y);
            instance.GetComponentInChildren<InputContainer>().InputName = name;
            instance.GetComponentInChildren<InputContainer>().SetName();
            instance.GetComponentInChildren<Image>().color = ((i % 2) == 0 ? ColourA : ColourB);
            y -= 40;
            inputs[i] = instance;
            i++;
        }

        this.GetComponent<RectTransform>().sizeDelta = new Vector2(0, -y);
    }

    public void RemoveAllObjects()
    {
        foreach(GameObject o in inputs)
        {
            Destroy(o);
        }
    }

    public void Back()
    {
        InputScreen.GetComponentInParent<InputScreen>().ShowScreen = false;
        // Save?
    }

    public void Save()
    {
        InputManager.SaveKeyBindings();
    }
}