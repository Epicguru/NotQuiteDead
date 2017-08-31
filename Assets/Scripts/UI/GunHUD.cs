using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GunHUD : MonoBehaviour {

    public static GunHUD Instance;
    public bool Open;
    public float ClosedX;
    public AnimationCurve Curve;
    public float OpenDuration;
    public Text Text;
    public Color Red, BlinkRed;

    private float timer2;
    private float timer;
    private RectTransform rect;
    private Vector2 pos = new Vector2();
    private Gun holding;

    private float BulletWarningPercentage = 0.3f;
    private int BulletWarningCount = 1;

    public void Start()
    {
        rect = transform as RectTransform;
        Instance = this;
    }

    public void Update()
    {
        timer += Time.deltaTime * (Open ? 1 : -1);

        if (timer < 0)
            timer = 0;
        if (timer > OpenDuration)
            timer = OpenDuration;

        float p = timer / OpenDuration;
        float curvedP = Curve.Evaluate(p);

        float x = Mathf.LerpUnclamped(ClosedX, 0, curvedP);

        pos.Set(x, 0);
        rect.anchoredPosition = pos;

        Open = holding != null && !PlayerInventory.IsOpen && !CommandInput.Instance.Open;

        Crosshair.Instance.Active = Open;
        if (holding != null)
        {
            Crosshair.Instance.MaxDistance = holding.Shooting.Damage.Inaccuracy.y;
            Crosshair.Instance.Distance = holding.Shooting.GetCurrentInaccuracy();
        }
        else
        {
            Crosshair.Instance.Distance = 0;
        }

        UpdateText();
    }

    public void SetHolding(Gun holding)
    {
        this.holding = holding;
    }

    private void UpdateText()
    {
        if (holding == null)
            return;
        timer2 += Time.deltaTime * 2;
        string text = GetText();
        if(Text.text != text)
        {
            Text.text = text;
        }
    }

    private string GetText()
    {
        if (holding == null)
            return string.Empty;

        string s = RichText.InColour(holding.Item.Name, Color.white);
        float percent = (float)holding.Shooting.bulletsInMagazine / (float)holding.Shooting.Capacity.MagazineCapacity;
        bool isLow = percent <= BulletWarningPercentage || holding.Shooting.bulletsInMagazine <= BulletWarningCount;
        Color colour = Text.color;
        if (isLow)
        {
            bool isBlink = ((int)timer2) % 2 == 0;
            colour = isBlink ? BlinkRed : Red;
        }
        string maxMagString = holding.Shooting.Capacity.MagazineCapacity + "";
        string ammoString = string.Empty + (holding.Shooting.bulletsInMagazine + (holding.Shooting.bulletInChamber ? 1 : 0));

        while (ammoString.Length < maxMagString.Length)
        {
            ammoString = "0" + ammoString;
        }
        s += "\n" + RichText.InColour(ammoString, colour) + "/" + maxMagString;
        s += "\n" + holding.Shooting.FiringMode;

        return s;
    }
}
