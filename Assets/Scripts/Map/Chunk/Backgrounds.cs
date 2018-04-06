
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Backgrounds : MonoBehaviour
{
    public static Backgrounds Instance;

    public static Dictionary<string, Background> Loaded;

    [SerializeField]
    private List<Background> loaded;

    public static Background GetBG(string prefab)
    {
        if (Loaded == null || !Loaded.ContainsKey(prefab))
            return null;

        return Loaded[prefab];
    }

    public void Awake()
    {
        Instance = this;
        MakeDictionary();
    }

    public void OnDestroy()
    {
        Instance = null;
    }

    public void Update()
    {
        if (Application.isEditor)
        {
            UpdateOrders();
        }
    }

    private void MakeDictionary()
    {
        if(Loaded == null)
        {
            Loaded = new Dictionary<string, Background>();
        }
        Loaded.Clear();

        if (loaded == null)
            return;

        foreach (var item in loaded)
        {
            if (Loaded.ContainsKey(item.Prefab))
            {
                Debug.LogError("Duplicate chunk background prefab ID: '{0}'! Name is '{1}' Skipped, original kept.".Form(item.Prefab, item.Name));
                continue;
            }
            else
            {
                Loaded.Add(item.Prefab, item);
            }
        }
    }

    public void UpdateOrders()
    {
        int index = 0;
        foreach (var item in loaded)
        {
            item.Order = index;
            index++;
        }
    }
}