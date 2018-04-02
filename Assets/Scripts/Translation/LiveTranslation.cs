using UnityEngine;

public class LiveTranslation : MonoBehaviour
{
    public int TranslationsPerSecond;
    private float timer = 0f;

	public void Update ()
	{
        timer += Time.unscaledDeltaTime;
        if(timer >= 1f)
        {
            timer -= 1f;
            TranslationsPerSecond = Translation.TranslationCounter;
            Translation.TranslationCounter = 0;
        }
        DebugText.Log("Current Language: " + Translation.GetCurrentLanguageVerbose());
        DebugText.Log("Translations Per Second: ~" + TranslationsPerSecond);
    }
}