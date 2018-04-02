using UnityEngine;
using UnityEngine.UI;

public class LoadingText : MonoBehaviour
{
    public Text Text;
    public int NormalSize;
    public int LargerSize;

    [Header("Controls")]
    public string RealityName;
    public int RealityDay;
    public float Percentage;

    public void Update()
    {
        Text.fontSize = NormalSize;
        Percentage = Mathf.Clamp(Percentage, 0f, 100f);
        Text.text = "<size=" + LargerSize + ">" + RealityName.Trim() + " : " + "General_Day".Translate().ToUpper() + " " + RealityDay + "</size>\n" + "General_Loading".Translate().ToUpper() + "\n" + Percentage.ToString("0") + "%";
    }
}