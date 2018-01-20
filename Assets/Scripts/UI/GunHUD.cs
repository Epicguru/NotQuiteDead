using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GunHUD : MonoBehaviour {

    public static GunHUD Instance;

    public Text Title;
    public Text FireMode;
    public Text AmmoText;
    public Image Icon;

    public bool Open;
    public float ClosedX;
    public AnimationCurve Curve;
    public float OpenDuration;
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

        bool benchOpen = Workbench.Bench == null ? false : Workbench.Bench.Open;
        Open = holding != null && !PlayerInventory.IsOpen && !CommandInput.Instance.Open && !benchOpen;

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
        SetText();
    }

    private void SetText()
    {
        if (holding == null)
        {
            AmmoText.text = "-/-";
            FireMode.text = "---";
            Title.text = "---";
            Icon.sprite = null;
            return;
        }

        // Title - Name
        string name = holding.Item.Name;

        // Ammo counter.
        float percent = holding.Shooting.Capacity.MagazineCapacity <= 3 ? 0f : ((float)holding.Shooting.bulletsInMagazine / (float)holding.Shooting.Capacity.MagazineCapacity);
        bool isLow = percent <= BulletWarningPercentage || holding.Shooting.bulletsInMagazine <= BulletWarningCount;

        Color colour = AmmoText.color;
        if (isLow)
        {
            bool isBlink = ((int)timer2) % 2 == 0;
            colour = isBlink ? BlinkRed : Red;
        }

        string maxMagString = holding.Shooting.Capacity.MagazineCapacity.ToString();
        string ammoString = string.Empty + (holding.Shooting.bulletsInMagazine + (holding.Shooting.bulletInChamber ? 1 : 0));

        while (ammoString.Length < maxMagString.Length)
        {
            ammoString = '0' + ammoString;
        }

        string ammo = RichText.InColour(ammoString, colour) + '/' + maxMagString;

        // Firing mode.
        string fireMode = holding.Shooting.FiringMode.ToString();

        //Apply all
        AmmoText.text = ammo;
        FireMode.text = fireMode;
        Title.text = name;
        Icon.sprite = holding.Item.ItemIcon;
    }
}
