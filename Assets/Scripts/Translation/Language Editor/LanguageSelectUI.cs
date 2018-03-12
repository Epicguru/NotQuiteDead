using System.Collections.Generic;
using UnityEngine;

public class LanguageSelectUI : MonoBehaviour
{
    public RectTransform Content;
    public LanguageSelectItemUI Prefab;

    public List<string> Langs;

    private List<GameObject> spawned = new List<GameObject>();

    public void Start()
    {
        SpawnAll(Langs.ToArray());
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
}