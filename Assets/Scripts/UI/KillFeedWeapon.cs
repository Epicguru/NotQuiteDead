using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class KillFeedWeapon : MonoBehaviour {

    public float TargetHeight = 30f;
    public float MaxWidth = 200f;

	public void Update()
    {
        float heightToForcedHeight = TargetHeight / GetComponent<Image>().sprite.textureRect.height;
        float width = Mathf.Clamp(GetComponent<Image>().sprite.textureRect.width * heightToForcedHeight, 0f, MaxWidth);

        // Set width using rect transform
        (transform as RectTransform).sizeDelta = new Vector2(width, TargetHeight);
    }
}
