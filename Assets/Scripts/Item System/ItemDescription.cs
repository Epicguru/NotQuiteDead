using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[System.Serializable]
public class ItemDescription
{
    [Tooltip("A short description of the item. Best one line.")]
    [TextArea(1, 2)]
    public string ShortDescription;

    [Tooltip("A full, detailed description of the item. As many lines as you want.")]
    [TextArea(5, 10)]
    public string LongDescription;
}
