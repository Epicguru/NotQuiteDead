using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform), typeof(Text))]
public class DayMessage : MonoBehaviour
{
    private static DayMessage instance;

    [Header("General")]
    public bool Display;
    public Color NormalColour;
    public Color FadedColour;
    public float TimeBeforeFade = 2f;
    public float FadeTime = 0.5f;
    [SerializeField]
    [ReadOnly]
    private bool Visible;

    [Header("Day Counter")]
    public int Day = 123;
    public float CountupTime = 1f;
    public AnimationCurve CountupCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    public float MinBeatInterval = 0.1f;

    [Header("Bump Effect")]
    public bool IncreaseNow;
    public float IncreasedScale = 1.3f;
    public float NormalScale= 1f;
    public float LerpSpeed = 2f;

    private float currentScale;
    private RectTransform rect;
    private Text text;
    private float timer;
    private bool countingUp = false;
    private int oldDay;
    private float timeSinceLastBeat;
    private float timeSinceDone;

    public void Awake()
    {
        instance = this;
    }

    public void OnDestroy()
    {
        instance = null;
    }

    public void Start ()
	{
        rect = GetComponent<RectTransform>();
        text = GetComponent<Text>();
    }
	
	public void Update ()
	{
        UpdateDayCounter();
        UpdateBumpEffect();
	}

    private void UpdateDayCounter()
    {
        if (Display)
        {
            timer = 0f;
            Display = false;
            oldDay = 0;
            timeSinceLastBeat = 0f;
            timeSinceDone = 0f;

            text.color = NormalColour;

            // Time to count up:
            float min = 0.3f;
            float max = 1f;
            CountupTime = Mathf.Clamp((Day / 100f) * 1f, min, max);

            countingUp = true;
        }

        timer += Time.unscaledDeltaTime;
        timer = Mathf.Clamp(timer, 0f, CountupTime);

        if(timer == CountupTime && countingUp)
        {
            // Done counting up!
            // TODO hide!

            text.text = "Day " + Day;
            IncreaseNow = true;
            countingUp = false;

            return;
        }
        else
        {
            if(timer == CountupTime)
            {
                timeSinceDone += Time.unscaledDeltaTime;

                if(timeSinceDone > TimeBeforeFade)
                {
                    float y = (timeSinceDone - TimeBeforeFade) / FadeTime;
                    Color c = Color.Lerp(NormalColour, FadedColour, y);
                    text.color = c;
                }
                return;
            }
        }
        if (!countingUp)
        {
            return;
        }

        // Set current day value...
        float p = Mathf.Clamp(timer / CountupTime, 0f, 1f);
        float x = Mathf.Clamp(CountupCurve.Evaluate(p), 0f, 1f);
        int day = Mathf.FloorToInt(x * Day);

        // Do beats...
        timeSinceLastBeat += Time.unscaledDeltaTime;
        if(oldDay != day)
        {
            oldDay = day;
            if(timeSinceLastBeat >= MinBeatInterval)
            {
                timeSinceLastBeat -= MinBeatInterval;
                IncreaseNow = true;
            }
        }

        string dayString = day.ToString();
        while(dayString.Length < Day.ToString().Length)
        {
            dayString = "0" + dayString;
        }
        text.text = "Day " + dayString;
    }

    private void UpdateBumpEffect()
    {
        // Bump up the current scale if increase now is true.
        if (IncreaseNow)
        {
            currentScale = IncreasedScale;
            IncreaseNow = false;
        }

        // Move current scale to match normal scale.
        currentScale = Mathf.Lerp(currentScale, NormalScale, Time.unscaledDeltaTime * LerpSpeed);

        // Set the scale of the object.
        rect.localScale = new Vector3(currentScale, currentScale, 1f);
    }

    public static void DisplayDay(int day)
    {
        if(instance != null)
        {
            instance.Day = day;
            instance.Display = true;
        }
    }
}