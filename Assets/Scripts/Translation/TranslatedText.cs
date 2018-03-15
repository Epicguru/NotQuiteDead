using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class TranslatedText : MonoBehaviour
{
    public string Key;
    public TranslatedTextMode Mode = TranslatedTextMode.START;

    private Text Text;

    private void Start()
    {
        Translation.OnLanguageChange.AddListener(LanguageChange);

        if(Mode == TranslatedTextMode.START || Mode == TranslatedTextMode.SCRIPT_AND_START)
        {
            UpdateText();
        }
    }

    private void Update()
    {
        if(Mode == TranslatedTextMode.UPDATE)
        {
            UpdateText();
        }
    }

    public void UpdateText()
    {
        Text.text = Key.Translate();
    }

    private void OnDestroy()
    {
        Translation.OnLanguageChange.RemoveListener(LanguageChange);
    }

    private void LanguageChange()
    {
        UpdateText();
    }
}

public enum TranslatedTextMode
{
    START,
    UPDATE,
    SCRIPT,
    SCRIPT_AND_START
}