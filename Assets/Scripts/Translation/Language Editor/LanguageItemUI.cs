using UnityEngine;
using UnityEngine.UI;

public class LanguageItemUI : MonoBehaviour
{
    [HideInInspector]
    public Language Lang;
    [HideInInspector]
    public LanguageDefinition Def;

    [Header("Controls")]
    public string Key;
    public Vector2 InputSizes = new Vector2(50f, 200f);
    public float TransitionTime = 0.5f;
    public AnimationCurve TransitionCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    [Header("State Colours")]
    public Color Translated = Color.green;
    public Color NotTranslated = Color.red;
    public Color Warning = Color.yellow;

    [Header("References")]
    public InputField ValueInput;
    public RectTransform ValueInputRect;
    public Toggle UseDefaultToggle;
    public Text KeyText;
    public Image Background;
    public Text StateText;

    private float timer;
	
	public void Update ()
	{
        // Set key text.
        KeyText.text = Key;

        if (UseDefaultToggle.isOn)
        {
            // Using default language, but don't destroy the current text.
            // Stop editing of value.
            ValueInput.interactable = false;
        }
        else
        {
            // Uses custom value.
            ValueInput.interactable = true;
        }

        // Update input size...
        // If it is selected and we are NOT using the default value.
        bool open = ValueInput.isFocused && !UseDefaultToggle.isOn;

        timer += Time.unscaledDeltaTime * (open ? 1f : -1f);
        timer = Mathf.Clamp(timer, 0f, TransitionTime);
        float p = timer / TransitionTime;
        // Where p == 1 when it is completely open.
        float x = TransitionCurve.Evaluate(p);

        float ySize = Mathf.Lerp(InputSizes.x, InputSizes.y, x);

        // Set real size.
        ValueInputRect.sizeDelta = new Vector2(320, ySize);

        UpdateState();
    }

    private void UpdateState()
    {
        // Translated is true when a non-null, non-whitespace value is present for the key.
        bool translated = Lang.KeyIsTranslated(Key);

        // Missing is if the key is not even present in the language.
        bool missing = !Lang.ContainsKey(Key);

        // Warning is if the default value is used or if the value is undefined.
        bool warning = UseDefaultToggle.isOn || !Def.ContainsKey(Key);

        StateText.text = "---";

        if (missing)
        {
            Background.color = NotTranslated;
            StateText.text = "Not saved!";
        }
        else
        {
            if (warning)
            {
                Background.color = Warning;
                if(UseDefaultToggle.isOn)
                    StateText.text = "Uses default language!";
                else
                    StateText.text = "This key is not defined in the game!";

            }
            else
            {
                Background.color = Translated;
                StateText.text = "Translated!";
            }
        }
    }
}