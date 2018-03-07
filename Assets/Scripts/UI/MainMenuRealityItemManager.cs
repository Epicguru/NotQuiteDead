using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MainMenuRealityItemManager : MonoBehaviour
{
    public MainMenuReality Prefab;
    public MainMenuRealityDetails Details;

    private List<GameObject> spawned = new List<GameObject>();

    public void OnEnable()
    {
        RefreshWithFolderContents();
    }

    public void RefreshWithFolderContents()
    {
        // Refreshes getting the name of all folders within the realities save folder.
        string path = OutputUtils.RealitySaveDirectory;
        Debug.Log("Getting reality names from '" + path + "'");

        if (!Directory.Exists(path))
        {
            Debug.LogWarning("Path to the realites '" + path + "' does not exist!");
            RefreshItems(null);
            return;
        }

        string[] folders = Directory.GetDirectories(path);
        for (int i = 0; i < folders.Length; i++)
        {
            string s = folders[i];

            folders[i] = Path.GetFileName(s);
        }

        // TODO could filter here or something...

        RefreshItems(folders);
    }

    public void RefreshItems(params string[] realities)
    {
        DestroySpawned();

        if (realities == null)
            return;

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