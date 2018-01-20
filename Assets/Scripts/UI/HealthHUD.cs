using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthHUD : MonoBehaviour {

    public RectTransform Bounds;
    public RectTransform Bar;
    public Image BarImage;

    public AnimationCurve LerpCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public float LerpTime = 0.2f;

    public Text Text;

    public Color HealthBarNormal;
    public Color HealthBarDamage;
    public Color HealthBarHeal;

    private float timer = 10f;
    private float oldHealth;
    private float lerpFrom;

    public void Update()
    {
        if(Player.Local != null)
        {
            float currentHealth = Player.Local.Health.GetHealth();
            if (oldHealth != currentHealth)
            {
                timer = 0;
                lerpFrom = oldHealth;
            }
            oldHealth = currentHealth;

            // Set Text value.
            int value = Mathf.CeilToInt(currentHealth);
            Text.text = (value >= 0 ? "+" : "-") + value.ToString();

            // Update health bar UI.
            UpdateBarWidth();
        }
    }

    private float TotalWidth()
    {
        return Bounds.sizeDelta.x;
    }

    private void UpdateBarWidth()
    {
        timer += Time.unscaledDeltaTime;

        float p = Mathf.Clamp(timer / LerpTime, 0f, 1f);

        if(timer >= LerpTime)
        {
            SetBarFill(CurrentHealthPercentage());
            SetBarColour(HealthBarNormal);
        }
        else
        {
            float curveValue = Mathf.Clamp(LerpCurve.Evaluate(p), 0f, 1f);
            float interpolated = Mathf.Lerp(lerpFrom, Player.Local.Health.GetHealth(), curveValue);
            float finalValue = interpolated / Player.Local.Health.GetMaxHealth();

            bool healing = lerpFrom < Player.Local.Health.GetHealth();

            Color c;
            if (healing)
            {
                c = Color.Lerp(HealthBarHeal, HealthBarNormal, p);
            }
            else
            {
                c = Color.Lerp(HealthBarDamage, HealthBarNormal, p);
            }

            SetBarColour(c);
            SetBarFill(finalValue);
        }
    }

    private float CurrentHealthPercentage()
    {
        return Player.Local.Health.GetHealthPercentage();
    }

    private void SetBarColour(Color colour)
    {
        BarImage.color = colour;
    }

    private void SetBarFill(float p)
    {
        // p == 1 means that it is full.
        // p == 0 means that it is empty.

        // Clamp, although the mask would hide it if it went out of bounds.
        p = Mathf.Clamp(p, 0f, 1f);

        // The total width of the health bar.
        float width = TotalWidth();

        // The real inverted width for the bar.
        float realWidth = width * (1f - p);

        Bar.offsetMin = new Vector2(realWidth, 0f);
    }
}
