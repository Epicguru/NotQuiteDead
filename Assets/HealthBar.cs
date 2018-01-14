using UnityEngine;

public class HealthBar : MonoBehaviour
{
    public Color Health;
    public Color NoHealth;
    public Color DamagedHealth;

    public float MaxValue;
    public float CurrentValue;

    public bool Lerp = true;
    public AnimationCurve Curve;
    public float LerpTime = 0.2f;
    public bool DynamicHide = true;
    public float ShowTime = 7f;

    public SpriteRenderer OutlineSprite;
    public SpriteRenderer HealthSprite;
    public SpriteRenderer NoHealthSprite;

    private float timer;
    private float oldHealth;
    private float lerpFrom;

    public void Start()
    {
        oldHealth = CurrentValue;
    }

    public void Update()
    {
        timer += Time.deltaTime;

        OutlineSprite.enabled = IsShowing();
        HealthSprite.enabled = IsShowing();
        NoHealthSprite.enabled = IsShowing();

        DetectChange();
        float value = GetLerpedValue();

        // Calculate width of health bar.
        // A width of 2 is for 100 health, work from there.
        float width = 2f * (MaxValue / 100f);

        // Apply that width.
        OutlineSprite.size = new Vector2(width, OutlineSprite.size.y);

        // Center the whole health bar.
        Vector3 pos = OutlineSprite.gameObject.transform.localPosition;
        pos.x = (width / 2f) * -1f;
        OutlineSprite.gameObject.transform.localPosition = pos;

        // Apply width to the background sprite.
        NoHealthSprite.size = new Vector2(width - 0.1f, NoHealthSprite.size.y);

        // Apply width to the active health sprite.
        float w = Mathf.Clamp(2f * (value / 100f) - 0.1f, 0f, float.MaxValue);
        HealthSprite.size = new Vector2(w, HealthSprite.size.y);

        // Apply colour to the health sprite.
        HealthSprite.color = GetLerpedColour();
    }

    private float GetLerpedValue()
    {
        if (!Lerp)
        {
            return CurrentValue;
        }

        if(timer <= LerpTime)
        {
            float lerp = GetCurveEvaluation();
            float value = Mathf.LerpUnclamped(CurrentValue, lerpFrom, lerp);
            return value;
        }
        else
        {
            return CurrentValue;
        }
    }

    private Color GetLerpedColour()
    {
        if (!Lerp)
            return Health;

        if (!(timer <= LerpTime))
            return Health;

        if (lerpFrom <= CurrentValue)
            return Health;

        float x = 1f - Mathf.Clamp(timer / LerpTime, 0f, 1f);

        Color c = Color.Lerp(Health, DamagedHealth, x);

        return c;
    }

    private float GetCurveEvaluation()
    {
        float p = Mathf.Clamp(timer / LerpTime, 0f, 1f);
        float lerp = Curve.Evaluate(p);
        return lerp;
    }

    private void DetectChange()
    {
        if(oldHealth != CurrentValue)
        {
            timer = 0;
            lerpFrom = oldHealth;
            oldHealth = CurrentValue;
        }
    }

    public bool IsShowing()
    {
        return !DynamicHide || timer <= ShowTime;
    }
}