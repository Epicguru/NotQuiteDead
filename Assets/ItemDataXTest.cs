using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDataXTest : MonoBehaviour
{
    public ItemDataX ItemData;

    public void Start()
    {
        ItemData = new ItemDataX();
        ItemData.Created = true;
        ItemData.Add("XYZ", 123);
        ItemData.Add("Vector3", new Vector3(1f, 2f, 3f));
        ItemData.Add("James", "A cool person, yea!");
        ItemData.Add("Array Name Here", new float[] { 10f, 20f, 50f, 100f, 0.01f });
        ItemData.Add("A colour!", new Color(0.5f, 0.5f, 1f, 1f));

        JsonSerializerSettings settings = new JsonSerializerSettings();

        settings.Formatting = Formatting.Indented;
        settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
        settings.ContractResolver = UnityContractResolver.Instance;

        Debug.Log(JsonConvert.SerializeObject(ItemData, settings));
    }
}