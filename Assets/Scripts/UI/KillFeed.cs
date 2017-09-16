using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class KillFeed : NetworkBehaviour {

    public static KillFeed Instance;

    public KillFeedObject Prefab;
    public float MinTime = 5f;
    public float TargetScroll;
    public List<KillFeedAnimation> objects = new List<KillFeedAnimation>();

    private float Scroll;

    public void Start()
    {
        Instance = this;
    }

    private List<KillFeedAnimation> bin = new List<KillFeedAnimation>();
    public void Update()
    {
        foreach(KillFeedAnimation a in objects)
        {
            if(a.TimeEnabled >= MinTime)
            {
                a.Enabled = false;                
            }
            else
            {
                //a.Enabled = true;
            }
            if (a.Closed)
            {
                // Remove from list and move all others up!
                // Move up:
                TargetScroll += 30f;
                // Remove:
                bin.Add(a);
            }
        }

        foreach(KillFeedAnimation a in bin)
        {
            objects.Remove(a);
            Destroy(a.gameObject);
        }
        bin.Clear();

        Scroll += Mathf.Clamp(TargetScroll - Scroll, 0f, Time.deltaTime * 30f * 10f);

        (transform as RectTransform).anchoredPosition = new Vector2(0, Scroll);
    }

    public void AddKill(string killer, string killed, string item)
    {
        Item i = Item.FindItem(item);
        Sprite sprite = i == null ? null : i.ItemIcon;

        KillFeedObject spawned = Instantiate(Prefab, transform);

        spawned.Killer = killer;
        spawned.Killed = killed;
        spawned.Icon = sprite;

        (spawned.transform as RectTransform).anchoredPosition = new Vector2(1000, -5 - (TargetScroll + (30f * objects.Count)));

        objects.Add(spawned.GetComponent<KillFeedAnimation>());
    }
}
