
using UnityEngine;
using UnityEngine.UI;

public class DetailsStatistic : MonoBehaviour
{
    [SerializeField]
    private Image Icon;

    [SerializeField]
    private Text Stat;

    [SerializeField]
    private Text Value;

    public void Set(DetailStat stat)
    {
        Set(stat.Icon, stat.Key, stat.Value);
    }

    public void Set(Sprite icon, string name, string value)
    {
        SetIcon(icon);
        SetStatName(name);
        SetValue(value);
    }

    public void SetIcon(Sprite sprite)
    {
        Icon.sprite = sprite;
    }

    public void SetStatName(string name)
    {
        if (name == null)
        {
            Stat.text = "---";
            return;
        }
        Stat.text = name.Trim() + ':';
    }

    public void SetValue(string text)
    {
        Value.text = text;
    }
}

public struct DetailStat
{
    public Sprite Icon;
    public string Key;
    public string Value;
}