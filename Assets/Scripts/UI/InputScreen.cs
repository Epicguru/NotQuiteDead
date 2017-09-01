using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputScreen : MonoBehaviour {

    public static InputScreen Instance;
    public bool ShowScreen;
    private bool showing;

    public void Start()
    {
        Instance = this;
    }

    public void Update()
    {
        if(InputManager.InputDown("Key Config"))
        {
            ShowScreen = !ShowScreen;
        }
        if (ShowScreen)
        {
            Show();
        }
        else
        {
            Hide();
        }
    }

	public void Show()
    {
        if (showing)
            return;

        showing = true;
        GetComponentsInChildren<Transform>(true)[1].gameObject.SetActive(true);
        InputManagerGUI.Instance.PlaceAllObjects();
    }

    public void Hide()
    {
        if (!showing)
            return;

        showing = false;
        GetComponentsInChildren<Transform>(true)[1].gameObject.SetActive(false);
        InputManagerGUI.Instance.RemoveAllObjects();
    }
}