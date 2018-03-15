using UnityEngine;
using UnityEngine.UI;

public class LanguageDefinitionItemUI : MonoBehaviour
{
    public Image BackgroundImage;
    public Image LockedImage;
    public Toggle Lock;
    public InputField NameInput;
    public InputField DescriptionInput;
    public InputField ParamsInput;
    public Text ErrorText;
    public Color NormalColour;
    public Color ErrorColour;

    public void DeleteButtonPressed()
    {
        Destroy(this.gameObject);
    }

    public void Update()
    {
        LockedImage.enabled = Lock.isOn;

        UpdateError();
    }

    public void UpdateError()
    {
        string error = GetErrors();
        bool isError = error != null;

        if (isError)
        {
            ErrorText.text = error;
            BackgroundImage.color = ErrorColour;
        }
        else
        {
            ErrorText.text = "";
            BackgroundImage.color = NormalColour;
        }
    }

    public string GetErrors()
    {
        // Check for invalid name:
        if (string.IsNullOrWhiteSpace(NameInput.text.Trim()))
        {
            return "Name is empty!";
        }
        // TODO check for duplicate name.

        return null;
    }
}