using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Workbench : MonoBehaviour
{
    public static Workbench Bench;
    public Transform Container;
    public RectTransform Contents;
    public Button Craft;
    public bool Open
    {
        get
        {
            return _Open;
        }
        set
        {
            if (value == _Open)
                return;
            _Open = value;
            OpenChange();
        }
    }
    private bool _Open;
    public Sprite[] Backgrounds;
    public Image Background;
    public Dropdown Tab;

    public BlueprintRequirementDisplay Requirements;
    public BlueprintsResults Results;
    public BlueprintPreviewDisplay Preview;
    public Transform ItemParent;
    public BlueprintItem ItemPrefab;

    public List<Blueprint> Blueprints = new List<Blueprint>();

    private List<BlueprintItem> items = new List<BlueprintItem>();
    public Blueprint CurrentBlueprint;

    public void Awake()
    {
        Bench = this;
        List<string> files = new List<string>();
        LoadFiles(files);
        Blueprints = new List<Blueprint>();
        MakeBlueprints(files, Blueprints);

        PlayerInventory.inv.Inventory.ContentsChange += RefreshInventory;

        PopulateAvailable();
    }

    public void OnDestroy()
    {
        if (Bench == this)
            Bench = null;
    }

    public void Update()
    {
        if(InputManager.InputDown("Escape", true))
        {
            Open = false;
        }
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

    private void OpenChange()
    {
        Container.gameObject.SetActive(Open);
        InputManager.Active = !Open;
        if (Open)
        {
            RefreshInventory();
        }
    }

    public void RefreshInventory()
    {
        foreach(BlueprintItem i in items)
        {
            i.CanCraft = i.Blueprint.PlayerHasMaterials();
        }
        Requirements.Refresh();
        Results.Refresh();
        if (CurrentBlueprint != null)
            Craft.interactable = CurrentBlueprint.PlayerHasMaterials();
        else
            Craft.interactable = false;
    }

    public void SetCurrentBlueprint(Blueprint b)
    {
        CurrentBlueprint = b;
        Craft.interactable = b.PlayerHasMaterials();
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

        Contents.anchoredPosition = new Vector2(0, 0);
        Contents.sizeDelta = new Vector2(0, 50 * i);

        RefreshInventory();
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
