using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class TranslatedText : MonoBehaviour
{
    public string Key;
    public TranslatedTextMode Mode = TranslatedTextMode.START;

    public TranslatedTextDetails Details;

    private Text Text;

    private void Start()
    {
        Text = GetComponent<Text>();

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
        string trans = Key.Translate();
        // Capitalize
        if (Details.Capitalize && !Details.Lowercase)
        {
            trans = trans.ToUpper();
        }
        if (Details.Lowercase)
        {
            trans = trans.ToLower();
        }
        if(Details.FirstCap && !Details.LowerFirstCap)
        {
            trans = trans.FirstCap();
        }
        if (Details.LowerFirstCap)
        {
            trans = trans.LowerFirstCap();
        }

        Text.text = trans;
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

[Serializable]
public class TranslatedTextDetails
{
    public bool Capitalize = false;
    public bool Lowercase = false;
    public bool FirstCap = false;
    public bool LowerFirstCap = false;
}