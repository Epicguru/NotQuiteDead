using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputScreen : MonoBehaviour {

    public bool ShowScreen;
    private bool showing;

    public void Update()
    {
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