
using UnityEngine;
using UnityEngine.EventSystems;

public class BuildingBarDrop : MonoBehaviour
{
    public BuildingBarUI Bar;

    public void Start()
    {
        EventTrigger trigger = GetComponent<EventTrigger>();
        EventTrigger.Entry entry;

        // Drop.
        entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.Drop;
        entry.callback.AddListener((data) => { Drop((PointerEventData)data); });
        trigger.triggers.Add(entry);
    }

    public void Drop(PointerEventData data)
    {
        string prefab = data.pointerDrag.GetComponent<BuildingMenuItem>().Prefab;

        Vector2 localPos = data.position - (Vector2)((RectTransform)transform).position;

        // Determine index from loca position...
        // General formula for item x position is:
        // x = 5 + 55 * index
        // x - 5 = 55 * index
        // (x - 5) / 55 = index
        // index = (x - 5) / 55
        // Algebra is actually useful, aparently.

        float x = localPos.x;
        int index = Mathf.RoundToInt((x - 5f) / 55f);

        // Make sure that the index is in bounds.
        index = Mathf.Clamp(index, 0, Bar.Items.Count);

        Bar.Items.Insert(index, Player.Local.BuildingInventory.GetItem(prefab));
        Bar.Refresh();
    }
}