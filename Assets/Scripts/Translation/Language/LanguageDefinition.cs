
using System;
using System.Collections.Generic;
using UnityEngine;

public class LanguageDefinition
{
    public List<LangDefParam> Data = new List<LangDefParam>();
}

[Serializable]
public struct LangDefParam
{
    public string Key;
    public int Params;
    [TextArea(2, 10)]
    public string Desription;
}