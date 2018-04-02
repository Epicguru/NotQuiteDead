using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class LanguageDefinitionUI : MonoBehaviour
{
    public RectTransform Content;
    public LanguageDefinitionItemUI Prefab;
    public InputField FilterInput;
    public Dropdown Defs;

    public Button SaveButton;
    public Button AddButton;

    public LanguageDefinition CurrentDefinition;

    private List<LanguageDefinitionItemUI> spawned = new List<LanguageDefinitionItemUI>();

    public void Start()
    {
        Defs.ClearOptions();
        if (Directory.Exists(LanguageIO.LanguageDefinitionFolder))
        {
            string[] files = Directory.GetFiles(LanguageIO.LanguageDefinitionFolder, "*.txt");
            for (int i = 0; i < files.Length; i++)
            {
                files[i] = Path.GetFileNameWithoutExtension(files[i]);
            }
            var list = new List<string>(files);
            list.Insert(0, "None");
            Defs.AddOptions(list);
        }
        DropdownOptionChanged();
    }

    public void DropdownOptionChanged()
    {
        if(CurrentDefinition != null)
        {
            SaveButtonPressed();
        }

        if(Defs.options[Defs.value].text != "None")
        {
            CurrentDefinition = LanguageIO.LoadDefinition(Defs.options[Defs.value].text);
            CurrentDefinition.Name = Defs.options[Defs.value].text;
            SpawnFromDef(CurrentDefinition);
        }
        else
        {
            CurrentDefinition = null;
            DestroySpawned();
        }
    }

    public void ApplyCurrentState()
    {
        if (CurrentDefinition == null)
            return;

        if(CurrentDefinition.Data == null)
        {
            CurrentDefinition.Data = new Dictionary<string, LangDefParam>();
        }
        else
        {
            CurrentDefinition.Data.Clear();
        }

        foreach (var item in spawned)
        {
            if (item == null)
                continue;

            string key = item.NameInput.text.Trim();
            string description = item.DescriptionInput.text.Trim();
            string[] args = item.ParamsInput.text.Trim().Replace("\n", "").Split(',');
            for (int i = 0; i < args.Length; i++)
            {
                args[i] = args[i].Trim();
            }
            if (CurrentDefinition.ContainsKey(key))
            {
                Debug.LogError("Duplicate key! [" + key + "]");
                continue;
            }
            else
            {
                LangDefParam value = new LangDefParam();
                value.Key = key;
                value.Params = args;
                value.Desription = description;
                value.Locked = item.Lock.isOn;
                CurrentDefinition.Data.Add(key, value);
            }
        }
    }

    public void SaveButtonPressed()
    {
        if(CurrentDefinition != null)
        {
            ApplyCurrentState();
            LanguageIO.SaveDefinition(CurrentDefinition);
        }
    }

    public void Update()
    {
        SaveButton.interactable = CurrentDefinition != null;
        AddButton.interactable = CurrentDefinition != null;

        Filter(FilterInput.text);
    }

    public void SpawnFromDef(LanguageDefinition def)
    {
        DestroySpawned();

        if (def == null)
            return;
        if (def.Data == null || def.Data.Count == 0)
            return;

        foreach (var pair in def.Data)
        {
            var spawn = Instantiate(Prefab, Content);

            spawn.NameInput.text = pair.Value.Key.Trim();
            spawn.DescriptionInput.text = pair.Value.Desription.Trim();
            spawn.Lock.isOn = pair.Value.Locked;
            string s = "";

            // Make params string.
            for (int i = 0; i < pair.Value.Params.Length; i++)
            {
                string str = pair.Value.Params[i];
                string sep = ",\n";
                if (i == pair.Value.Params.Length - 1)
                    sep = "";
                s += str + sep;
            }

            spawn.ParamsInput.text = s;

            spawned.Add(spawn);
        }
    } 

    public void Filter(string filter)
    {
        filter = filter.ToLower().Trim();
        bool allEnabled = string.IsNullOrWhiteSpace(filter);
        foreach (var item in spawned)
        {
            if(item != null)
            {
                if (allEnabled)
                {
                    if(!item.gameObject.activeSelf)
                        item.gameObject.SetActive(true);
                }
                else
                {
                    if (item.NameInput.text.Trim().ToLower().Contains(filter))
                    {
                        if (!item.gameObject.activeSelf)
                            item.gameObject.SetActive(true);
                    }
                    else
                    {
                        if (!item.NameInput.isFocused)
                        {
                            if (item.gameObject.activeSelf)
                                item.gameObject.SetActive(false);
                        }
                    }
                }
            }
        }
    }

    public void NewButtonPressed()
    {
        // Make a new ui item instance.
        var spawn = Instantiate(Prefab, Content);
        spawn.Lock.isOn = false;
        spawned.Add(spawn);
    }

    public void DestroySpawned()
    {
        foreach (var item in spawned)
        {
            if(item != null)
            {
                Destroy(item.gameObject);
            }
        }

        spawned.Clear();
    }
}