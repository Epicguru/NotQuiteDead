using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LanguageDefinitionUI : MonoBehaviour
{
    public RectTransform Content;
    public LanguageDefinitionItemUI Prefab;
    public InputField FilterInput;

    public LanguageDefinition CurrentDefinition;

    private List<LanguageDefinitionItemUI> spawned = new List<LanguageDefinitionItemUI>();

    public void Update()
    {
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