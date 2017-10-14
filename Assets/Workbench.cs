using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Workbench : MonoBehaviour
{
    public Sprite[] Backgrounds;
    public Image Background;
    public Dropdown Tab;

    public BlueprintPreviewDisplay Preview;
    public Transform ItemParent;
    public BlueprintItem ItemPrefab;

    public List<Blueprint> Blueprints = new List<Blueprint>();

    private List<BlueprintItem> items = new List<BlueprintItem>();

    public void Awake()
    {
        List<string> files = new List<string>();
        LoadFiles(files);
        Blueprints = new List<Blueprint>();
        MakeBlueprints(files, Blueprints);

        PopulateAvailable();
    }

    public void LoadFiles(List<string> files)
    {
        // From resources folder.
        TextAsset[] assets = Resources.LoadAll<TextAsset>("Blueprints");

        foreach(TextAsset t in assets)
        {
            files.Add(t.text);
        }
    }

    public void MakeBlueprints(List<string> files, List<Blueprint> blueprints)
    {
        foreach(string s in files)
        {
            Blueprint b = BlueprintLoader.GetBlueprint(s);
            if (b != null)
                blueprints.Add(b);
        }
    }

    public void PopulateAvailable()
    {
        int i = 0;
        foreach(Blueprint b in Blueprints)
        {
            BlueprintItem spawned = Instantiate<BlueprintItem>(ItemPrefab, ItemParent);
            (spawned.transform as RectTransform).anchoredPosition = new Vector2(0, -50 * i);
            spawned.Item = b.Products[0]; // First product is 'primary' product. Other products are secondary and the recipie is not 'theirs'.
            spawned.Blueprint = b;
            items.Add(spawned);
            i++;
        }
    }

    public void ClearOld()
    {
        foreach(BlueprintItem i in items)
        {
            Destroy(i);
        }
        items.Clear();
    }

    public void TabChange()
    {
        Background.sprite = Backgrounds[Tab.value];
    }
}
