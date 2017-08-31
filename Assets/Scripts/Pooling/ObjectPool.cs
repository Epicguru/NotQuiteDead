using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ObjectPool
{
    private static Transform IdleParent;
    private static Dictionary<PoolType, List<GameObject>> objects = new Dictionary<PoolType, List<GameObject>>();
    private static StringBuilder builder = new StringBuilder();

    public Transform idleParent;

    public void Start()
    {
        IdleParent = idleParent;
    }

    public static GameObject Instantiate(GameObject prefab, PoolType type)
    {
        return Instantiate(prefab, type, Vector3.zero, Quaternion.identity, null);
    }

    public static GameObject Instantiate(GameObject prefab, PoolType type, Vector3 position, Quaternion rotation, Transform parent)
    {
        GameObject borrowed = GetNewBorrowed(type); // Try to get an inactive object.
        if (borrowed == null) // If there is not one...
        {
            CreateNewIdle(prefab, type); // Create a new one!
            borrowed = GetNewBorrowed(type); // And get it.
        }

        borrowed.transform.position = position;
        borrowed.transform.rotation = rotation;

        borrowed.transform.SetParent(parent);

        return borrowed;
    }

    public static GameObject Instantiate(GameObject prefab, PoolType type, Transform parent)
    {
        return Instantiate(prefab, type, Vector3.zero, Quaternion.identity, parent);
    }

    public static void Destroy(GameObject obj, PoolType type)
    {
        ReturnBorrowed(obj, type);
    }

    public static string GetStats()
    {
        builder.Length = 0;
        float total = GetObjectCount();

        foreach (PoolType t in Enum.GetValues(typeof(PoolType)))
        {
            int c = GetObjectCount(t);
            int p = (int)(((float)c / total) * 100f);

            string percentage = p.ToString() + '%';
            while(percentage.Length < 4)
            {
                percentage = "0" + percentage;
            }

            string count = c.ToString();
            while(count.Length < 4)
            {
                count = "0" + count;
            }

            builder.Append(t.ToString().Replace('_', ' '));
            builder.Append(" - ");
            builder.Append(count);
            builder.Append(" ~ ");
            builder.Append(percentage);

            builder.Append('\n');
        }

        string s = builder.ToString();
        builder.Length = 0;

        return s;
    }

    public static int GetObjectCount()
    {
        int count = 0;
        foreach(List<GameObject> o in objects.Values)
        {
            count += o.Count;
        }

        return count;
    }

    public static int GetObjectCount(PoolType type)
    {
        if (!objects.ContainsKey(type))
        {
            return 0;
        }
        else
        {
            return objects[type].Count;
        }
    }

    public static void Clear()
    {
        foreach (PoolType t in objects.Keys)
        {
            Clear(t);
        }
    }

    public static void Clear(PoolType type)
    {
        // Removes all idle objects. Does not affect borrowed objects.
        Ensure(type);

        foreach(GameObject o in objects[type])
        {
            GameObject.Destroy(o);
        }

        objects[type].Clear();
    }

    private static void Ensure(PoolType type)
    {
        if (!objects.ContainsKey(type))
        {
            objects.Add(type, new List<GameObject>());
        }
    }

    private static void CreateNewIdle(GameObject prefab, PoolType type)
    {
        GameObject instance = GameObject.Instantiate(prefab);

        MakeBorrowed(instance);

        Ensure(type);
        objects[type].Add(instance);
    }

    private static void MakeIdle(GameObject o)
    {
        o.transform.SetParent(IdleParent);
        o.SetActive(false);
    }

    private static void ReturnBorrowed(GameObject obj, PoolType type)
    {
        if(obj == null)
        {
            Debug.LogError("Object to return to pool is null!");
            return;
        }

        MakeIdle(obj);
        Ensure(type);

        objects[type].Add(obj);
    }

    private static void MakeBorrowed(GameObject o)
    {
        // Do not set parent, this is does in accessors.
        o.SetActive(true);
    }

    private static GameObject GetNewBorrowed(PoolType type)
    {
        Ensure(type);

        List<GameObject> objs = objects[type];
        if (objs.Count == 0)
            return null;

        GameObject o = objs[0];
        while(o == null && objs.Count > 0)
        {
            Debug.LogError("Idle pooled object of type '" + type + "' was destroyed! Why?");
            objs.RemoveAt(0);
            o = objs[0];
        }

        MakeBorrowed(o);

        return o;
    }
}
