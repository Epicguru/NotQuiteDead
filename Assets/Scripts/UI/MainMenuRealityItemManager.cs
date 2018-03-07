using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuRealityItemManager : MonoBehaviour
{
    [Header("References")]
    public MainMenuReality Prefab;
    public MainMenuRealityDetails Details;
    public InputField RealityNameInput;
    public Button CreateButton;
    public Text InvalidNameText;

    [Header("New Reality")]
    public List<string> InvalidStrings;

    private List<GameObject> spawned = new List<GameObject>();
    private string[] currentRealities;

    public void OnEnable()
    {
        RealityNameInput.text = "";
        currentRealities = new string[0];
        RefreshWithFolderContents();
    }

    public void NewReality()
    {
        CreateNewReality(RealityNameInput.text.Trim());
        RealityNameInput.text = "";
    }

    public string IsNameValid(string name)
    {
        // Check for invalid characters.
        foreach (var invalid in InvalidStrings)
        {
            if (name.Contains(invalid))
            {
                return "Invalid name: Cannot contain character " + invalid;
            }
        }

        // Check for realities that already have that name.
        for (int i = 0; i < currentRealities.Length; i++)
        {
            string s = currentRealities[i];
            if(s.Trim() == name.Trim())
            {
                return "Invalid name: A reality already has that name!";
            }
        }

        return null;
    }

    public void Update()
    {
        bool typing = !(string.IsNullOrWhiteSpace(RealityNameInput.text));

        if (typing)
        {
            string error = IsNameValid(RealityNameInput.text.Trim());
            CreateButton.interactable = error == null;
            if(error != null)
            {
                InvalidNameText.text = error;
            }
            else
            {
                InvalidNameText.text = "";
            }
        }
        else
        {
            CreateButton.interactable = false;
            InvalidNameText.text = "";
        }
    }

    public void CreateNewReality(string name)
    {
        Debug.Log("Creating reality '" + name + "'...");

        string path = OutputUtils.RealitySaveDirectory + name.Trim();

        if (Directory.Exists(path))
        {
            Debug.LogError("Path for 'new' reality already exists, what?");
            return;
        }
        Directory.CreateDirectory(path);

        Debug.Log("Created at '" + path + "'.");

        // Refresh once it is saved.
        RefreshWithFolderContents();
    }

    public void RefreshWithFolderContents()
    {
        // Refreshes getting the name of all folders within the realities save folder.
        string path = OutputUtils.RealitySaveDirectory;

        if (!Directory.Exists(path))
        {
            Debug.LogWarning("Path to the realites '" + path + "' does not exist!");
            RefreshItems(null);
            currentRealities = new string[0];
            return;
        }

        string[] folders = Directory.GetDirectories(path);
        for (int i = 0; i < folders.Length; i++)
        {
            string s = folders[i];

            folders[i] = Path.GetFileName(s);
        }

        RefreshItems(folders);
    }

    public void RefreshItems(params string[] realities)
    {
        DestroySpawned();

        if (realities == null)
            return;

        this.currentRealities = realities;

        foreach (var s in realities)
        {
            if (!string.IsNullOrWhiteSpace(s))
            {
                Spawn(s, null);
            }
        }
    }

    public void Spawn(string realityName, Sprite icon)
    {
        // TODO implement icon.
        MainMenuReality r = Instantiate(Prefab, this.transform);
        r.Details = this.Details;
        r.RealityName = realityName;
        //r.Icon = icon;

        spawned.Add(r.gameObject);
    }

    public void DestroySpawned()
    {
        foreach (var go in spawned)
        {
            Destroy(go);
        }

        spawned.Clear();
    }
}