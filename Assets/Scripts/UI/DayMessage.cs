using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform), typeof(Text))]
public class DayMessage : MonoBehaviour
{
    [Header("Day Counter")]
    public int DisplayedDay = 123;

    [Header("Bump Effect")]
    public bool IncreaseNow;
    public float IncreasedScale = 1.3f;
    public float NormalScale= 1f;
    public float LerpSpeed = 2f;

    private float currentScale;
    private RectTransform rect;
    private Text text;

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
}