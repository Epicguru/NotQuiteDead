using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class LanguageSelectUI : MonoBehaviour
{
    public RectTransform Content;
    public LanguageSelectItemUI Prefab;
    public InputField NewLangIn;
    public Button NewButton;
    public Text NewLangError;

    public List<string> InvalidStrings = new List<string>();

    private List<GameObject> spawned = new List<GameObject>();

    public void Start()
    {
        SpawnFromFolderContents();
    }

    public void DestroySpawned()
    {
        foreach (var s in spawned)
        {
            Destroy(s);
        }
    }

    public void SpawnAll(params string[] languages)
    {
        DestroySpawned();

        foreach (var lang in languages)
        {
            var spawn = Instantiate(Prefab, Content);
            spawn.Language = lang;
            spawned.Add(spawn.gameObject);
        }
    }

    public string IsValidLangName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return "";
        }

        // TODO add checks!
        foreach (string invalid in InvalidStrings)
        {
            if (name.Contains(invalid))
            {
                return "Contains invalid string '" + invalid + "'";
            }
        }

        return null;
    }

    public void NewButtonPressed()
    {
        if(IsValidLangName(NewLangIn.text) == null)
        {
            string name = NewLangIn.text;
            NewLangIn.text = "";

            // Add new empty language file.
            var lang = new Language();
            lang.Name = name;

            LanguageIO.SaveLanguage(lang);

            Debug.Log("Added new language '" + name + "'");
            SpawnFromFolderContents();
        }
    }

    public void SpawnFromFolderContents()
    {
        string[] files = Directory.GetFiles(LanguageIO.LanguageFolder);

        for (int i = 0; i < files.Length; i++)
        {
            files[i] = Path.GetFileNameWithoutExtension(files[i]);
        }

        SpawnAll(files);
    }

    public void Update()
    {
        string nameError = IsValidLangName(NewLangIn.text);

        if(nameError == null)
        {
            NewButton.interactable = true;
            NewLangError.text = "";
        }
        else
        {
            NewLangError.text = nameError;
            NewButton.interactable = false;
        }
    }
}