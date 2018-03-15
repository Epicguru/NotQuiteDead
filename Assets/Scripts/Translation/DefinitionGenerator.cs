
using System.Collections.Generic;

public static class DefinitionGenerator
{
    public static void GenerateDefinitions(Dictionary<string, LangDefParam> dic)
    {
        // Item names and descriptions.
        GenItems(dic);
    }

    private static void GenItems(Dictionary<string, LangDefParam> dic)
    {
        // Auto generate item name and description definitions.
        if (Item.Items == null)
            Item.LoadItems();
        foreach (var item in Item.Items.Values)
        {
            // Descriptions.
            string key = item.Prefab + "_Desc";
            if (!dic.ContainsKey(key))
            {
                LangDefParam p = new LangDefParam();
                p.Key = key;
                p.Desription = "The description of the '" + item.Prefab + "' item. Please copy from English as accurately as possible.";
                dic.Add(key, p);
            }

            // Names
            key = item.Prefab + "_Name";
            if (!dic.ContainsKey(key))
            {
                LangDefParam p = new LangDefParam();
                p.Key = key;
                p.Desription = "The display name of the '" + item.Prefab + "' item.";
                dic.Add(key, p);
            }
        }
    }
}