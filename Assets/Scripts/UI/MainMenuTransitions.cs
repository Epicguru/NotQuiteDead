using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuTransitions : MonoBehaviour
{
    public RectTransform Menu, Other;

    public bool NotInMenu;
    public float TransitionTime = 1f;
    public AnimationCurve TransitionCurve;

    private float timer;

    public void Update()
    {
        // Update timers and stuff.
        UpdateTimer();

        // Calculate menu y pos;
        float y = GetProgress() * Screen.height;
        Menu.anchoredPosition = new Vector2(0f, y);

        y = GetProgress() * Screen.height - Screen.height;
        if(Other != null)
            Other.anchoredPosition = new Vector2(0f, y);

        UpdateActiveState();
    }

    public void UpdateTimer()
    {
        timer += Time.unscaledDeltaTime * (NotInMenu ? 1f : -1f);
        timer = Mathf.Clamp(timer, 0f, TransitionTime);
    }

    public float GetProgress()
    {
        float p = Mathf.Clamp(timer / TransitionTime, 0f, 1f);
        float x = TransitionCurve.Evaluate(p);
        return x;
    }

    public void UpdateActiveState()
    {
        if(Other != null)
        {
            if(timer == 0f)
            {
                Other.gameObject.SetActive(false);
            }
        }
    }
}