using UnityEngine;
using UnityEngine.UI;

public class LanguageSelectItemUI : MonoBehaviour
{
    public string Language;

    public Text Text;

	public void Start ()
	{
        Text.text = Language.Trim();
	}
	
	public void Selected()
    {
        Debug.Log("Clicked on language '" + Language + "'");
        GetComponentInParent<LanguageSelectUI>().LanguageClicked(Language);
    }
}