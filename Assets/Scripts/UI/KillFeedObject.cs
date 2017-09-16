using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class KillFeedObject : MonoBehaviour {

    public string Killer;
    public string Killed;
    public Sprite Icon;
    public Sprite Unknown;

    public Image Image;
    public Text KillerText, KilledText;

    public void Update()
    {
        if (Image == null || KilledText == null || KillerText == null)
            return;

        Image.sprite = Icon == null ? Unknown : Icon;
        KillerText.text = string.IsNullOrEmpty(Killer) ? "???" : Killer;
        KilledText.text = string.IsNullOrEmpty(Killed) ? "???" : Killed;
    }
}
