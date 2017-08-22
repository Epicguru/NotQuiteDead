using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputContainer : MonoBehaviour {

    public string InputName;

    public bool UseDisplayName = false;
    public string DisplayName;

    public void Start()
    {
        GetComponent<RectTransform>().anchoredPosition = new Vector2(0, GetComponent<RectTransform>().anchoredPosition.y);
    }

    public void SetName()
    {
        GetComponentInChildren<Text>().text = GetText();
    }

	public string GetText()
    {
        KeyCode c = InputManager.GetInput(InputName);
        return GetDisplayName() + "- " + c;
    }

    public void Set()
    {
        // Need to diaplay
        StartCoroutine(Test());
    }

    public IEnumerator Test()
    {
        InputCoverContainer.Instance.Cover.Show();
        yield return new WaitWhile(AnyInputDown);
        yield return new WaitWhile(NoInputsDown);
        KeyCode c = InputManager.GetAllKeysDown()[0];
        Debug.Log("Input '" + InputName + "' changed from '" + InputManager.GetInput(InputName) + "' to '" + c + "'");
        InputManager.ChangeInput(InputName, c);
        SetName();
        InputCoverContainer.Instance.Cover.Hide();
    }

    private bool AnyInputDown()
    {
        return InputManager.GetAllKeysDown().Length > 0;
    }

    private bool NoInputsDown()
    {
        return InputManager.GetAllKeysDown().Length == 0;
    }

    public void Reset()
    {
        InputManager.ResetInput(InputName);
        SetName();
    }

    public string GetDisplayName()
    {
        if (UseDisplayName)
            return DisplayName;
        else
            return InputName;
    }
}
