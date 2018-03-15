using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class LanguageSelectUI : MonoBehaviour
{
    public CurrentLanguageUI CurrentLang;
    public RectTransform Content;
    public LanguageSelectItemUI Prefab;
    public InputField NewLangIn;
    public Button NewButton;
    public Text NewLangError;

    public List<string> InvalidStrings = new List<string>();

    private List<GameObject> spawned = new List<GameObject>();

    public void Start()
    {
        foreach (var c in Path.GetInvalidFileNameChars())
        {
            InvalidStrings.Add(c.ToString());
        }
        SpawnFromFolderContents();
    }

    public void LanguageClicked(string lang)
    {
        LanguageDefinition def = new LanguageDefinition();
        def.Data.Add("SomeMenuThing", new LangDefParam() { Key = "SomeMenuThing", Desription = "Some menu botton that you really need to translate!" });
        def.Data.Add("ButtonLabel", new LangDefParam() { Key = "ButtonLabel", Desription = "Another botton that you really need to translate!" });
        def.Data.Add("ThingWithParams", new LangDefParam() { Key = "ThingWithParams", Desription = "Oooh! This one has params!", Params = new string[] { "Player Name", "Item Name" } });

        if(Item.Items == null)
        {
            Item.LoadItems();
        }

        foreach (var item in Item.Items.Values)
        {
            string key = item.Prefab + "_Desc";
            LangDefParam p = new LangDefParam();
            p.Key = key;
            p.Desription = "The description of this item. Please copy from English as accurately as possible but remove parts that have no logical translation into the target language.";
            def.Data.Add(key, p);
        }

        Language loaded = LanguageIO.LoadLanguage(lang);

        CurrentLang.SpawnAll(def, loaded);
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
        if (!Directory.Exists(LanguageIO.LanguageFolder))
            return;
        string[] files = Directory.GetFiles(LanguageIO.LanguageFolder);

        List<string> processed = new List<string>();

        foreach (string file in files)
        {
            if(Path.GetExtension(file) == ".txt")
            {
                processed.Add(Path.GetFileNameWithoutExtension(file));
            }
        }

        SpawnAll(processed.ToArray());
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