using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CurrentLanguageUI : MonoBehaviour
{
    public Transform Content;
    public LanguageItemUI Prefab;
    public InputField FilterInput;

    [HideInInspector]
    public Language CurrentLang;
    [HideInInspector]
    public LanguageDefinition CurrentDef;

    private List<LanguageItemUI> spawned = new List<LanguageItemUI>();

    public void SavePressed()
    {
        if(CurrentLang == null)        
            return;        

        Debug.Log("Saving '" + CurrentLang.ToString() + "'");
        ApplyStateToLang();
        LanguageIO.SaveLanguage(CurrentLang);
    }

    public void ApplyStateToLang()
    {
        if(CurrentLang == null)
        {
            return;
        }

        if(CurrentLang.Data == null)
        {
            CurrentLang.Data = new Dictionary<string, string>();
        }
        else
        {
            CurrentLang.Data.Clear();
        }

        foreach (var go in spawned)
        {
            LanguageItemUI item = go.GetComponent<LanguageItemUI>();
            string value = item.UseDefaultToggle.isOn ? (Language.IS_DEFAULT_VALUE + item.ValueInput.text.Trim()) : item.ValueInput.text.Trim();
            if (!CurrentLang.ContainsKey(item.Key))
            {
                CurrentLang.Data.Add(item.Key, value);
            }
        }
    }

    public void SpawnAll(LanguageDefinition def, Language lang)
    {
        // Spawns using a language definition and a real language...
        DestroySpawned();

        CurrentLang = lang;
        CurrentDef = def;

        if (def == null || def.Data == null || lang == null)
        {
            return;
        }
        if(lang.Data == null)
        {
            lang.Data = new Dictionary<string, string>();
        }

        // First spawn all values that are defined;
        foreach (var defined in def.Data)
        {
            LanguageItemUI item = Instantiate(Prefab, Content);
            item.Key = defined.Key;
            item.Lang = lang;
            item.Def = def;

            if (!lang.ContainsKey(defined.Key))
            {
                // The language does not contain a value for this definition key, set it to be default language version.
                item.UseDefaultToggle.isOn = true;
                Debug.Log("Setting up missing key '" + defined.Key + "' in lang '" + lang.ToString() + "'");
            }
            else
            {
                // Language already contains this key, but it could be default lang or whatever. Just look at the code and work it out.
                if (lang.IsDefaultLang(defined.Key))
                {
                    // This is a default translation! Toggle!
                    item.UseDefaultToggle.isOn = true;
                    // Extract the text that is written, even though it will never be seen in-game...
                    item.ValueInput.text = lang.Data[defined.Key].Replace(Language.IS_DEFAULT_VALUE, "").Trim();
                }
                else
                {
                    // This is not a default language item, set the real value!
                    item.UseDefaultToggle.isOn = false;

                    item.ValueInput.text = lang.Data[defined.Key];
                }
            }

            spawned.Add(item);
        }

        // Spawn all items within the language that are not defined but are present.
        foreach (var present in lang.Data)
        {
            string key = present.Key;
            if (!def.Data.ContainsKey(key))
            {
                LanguageItemUI item = Instantiate(Prefab, Content);
                item.Key = present.Key;
                item.Lang = lang;
                item.Def = def;
                item.UseDefaultToggle.isOn = false;
                item.ValueInput.text = present.Value.Replace(Language.IS_DEFAULT_VALUE, "");
                spawned.Add(item);
            }
  
        }
    }

    public void Update()
    {
        Filter(FilterInput.text);
    }

    public void Filter(string filter)
    {
        filter = filter.ToLower().Trim();
        bool allActive = string.IsNullOrWhiteSpace(filter);
        foreach (var item in spawned)
        {
            if (allActive || item.Key.ToLower().Contains(filter))
            {
                if(!item.gameObject.activeSelf)
                    item.gameObject.SetActive(true);
            }
            else
            {
                if (item.gameObject.activeSelf)
                    item.gameObject.SetActive(false);
            }
        }
    }

    public void DestroySpawned()
    {
        foreach (var item in spawned)
        {
            Destroy(item.gameObject);
        }
        spawned.Clear();
    }
}