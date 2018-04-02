using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CurrentLanguageInfoBoxUI : MonoBehaviour, IDragHandler
{
    public Text Text;

    public string DefaultLangVersion;
    public string Description;
    public string Params;

    public void Start()
    {
        (transform as RectTransform).anchoredPosition = new Vector2(Screen.width / 2f, Screen.height / 2f);
    }

    public void SetText()
    {
        string text;
        text = "<b>Default Version</b>\n" +
               DefaultLangVersion.Trim() + "\n\n" +
               "<b>Description</b>\n" +
               Description.Trim() + "\n\n" +
               "<b>Arguments</b>\n" +
               Params;

        Text.text = text;
    }

    public void ClampToScreen()
    {
        RectTransform rt = transform as RectTransform;
        Vector2 pos = rt.anchoredPosition;
        Vector2 size = rt.sizeDelta;

        float x = pos.x;
        float y = pos.y;
        float right = pos.x + size.x;
        float top = pos.y + size.y;

        if(right > Screen.width)
        {
            x = Screen.width - size.x;
        }
        if(top > Screen.height)
        {
            y = Screen.height - size.y;
        }
        x = Mathf.Max(x, 0f);
        y = Mathf.Max(y, 0f);

        rt.anchoredPosition = new Vector2(x, y);
    }

    public void Open()
    {
        SetText();
        gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    public void OnDrag(PointerEventData eventData)
    {
        (transform as RectTransform).anchoredPosition += eventData.delta;
        ClampToScreen();
    }

    public void Update()
    {
        ClampToScreen();
    }
}