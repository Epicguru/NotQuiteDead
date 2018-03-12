
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Language Definition", order = 3)]
public class LanguageDefinition : ScriptableObject
{
    public List<LangDefParam> Data = new List<LangDefParam>();
}

[Serializable]
public class LangDefParam
{
    public string Key;
    public string[] Params;
    [TextArea(2, 10)]
    public string Desription;
}