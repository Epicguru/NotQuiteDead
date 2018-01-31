﻿
using UnityEngine;

public class TacLight : MonoBehaviour
{
    public Attachment Attachment;
    public Disabler Disabler;

    public void Update()
    {
        Disabler.Enabled = Attachment.IsAttached && Attachment.Gun.GetComponent<Item>().IsEquipped();
    }
}