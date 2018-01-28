
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BuildingBarItem : MonoBehaviour
{
    [Header("Data (Code)")]
    public Sprite Icon;
    public string Name;
    public string Prefab;

    [Header("Data")]
    [SerializeField]
    [ReadOnly]
    private bool dragging;
    [SerializeField]
    [ReadOnly]
    private Vector3 oldPos;

    [Header("Editor")]
    [SerializeField]
    private Image image;
    [SerializeField]
    private Image background;
    [SerializeField]
    private Color defaultColour;
    [SerializeField]
    private Color selectedColour;

    public void Start()
    {
        EventTrigger trigger = GetComponent<EventTrigger>();
        EventTrigger.Entry entry;

        // Drag begin.
        entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.BeginDrag;
        entry.callback.AddListener((data) => { DragBegin((PointerEventData)data); });
        trigger.triggers.Add(entry);

        // Drag end.
        entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.EndDrag;
        entry.callback.AddListener((data) => { DragEnd((PointerEventData)data); });
        trigger.triggers.Add(entry);

        // Drag ongoing.
        entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.Drag;
        entry.callback.AddListener((data) => { DragOngoing((PointerEventData)data); });
        trigger.triggers.Add(entry);
    }

    public void DragBegin(PointerEventData data)
    {
        if (dragging)
            return;

        dragging = true;
        oldPos = transform.position;
    }

    public void DragOngoing(PointerEventData data)
    {
        if (!dragging)
            return;

        transform.position = data.position - ((RectTransform)transform).sizeDelta / 2f;
    }

    public void DragEnd(PointerEventData data)
    {
        if (!dragging)
            return;

        dragging = false;

        BuildingBarUI bar = GetComponentInParent<BuildingBarUI>();

        // Work out the old index.
        float oldX = oldPos.x;
        int oldIndex = Mathf.RoundToInt((oldX - 5f) / 55f);

        // Make sure that the index is in bounds.
        oldIndex = Mathf.Clamp(oldIndex, 0, bar.Items.Count);

        // Work out newly placed index...
        float x = ((RectTransform)transform).anchoredPosition.x;
        int index = Mathf.RoundToInt((x - 5f) / 55f);

        // Make sure that the index is in bounds.
        index = Mathf.Clamp(index, 0, bar.Items.Count);

        if (index == oldIndex)
        {
            transform.position = oldPos;
            return;
        }

        if(index > oldIndex)
        {
            // Update the bar.
            index += 1;
            index = Mathf.Clamp(index, 0, bar.Items.Count);
            bar.Items.Insert(index, Player.Local.BuildingInventory.GetItem(Prefab));
            bar.Items.RemoveAt(oldIndex);
            bar.Refresh();
        }
        else
        {
            // Update the bar.
            index = Mathf.Clamp(index, 0, bar.Items.Count);
            bar.Items.Insert(index, Player.Local.BuildingInventory.GetItem(Prefab));
            bar.Items.RemoveAt(oldIndex + 1);
            bar.Refresh();
        }

        // 0, 1, 2, 3
        // a, b, c, d

        // Moving from 2 to 1

        Debug.Log("Moved from " + oldIndex + " to " + index);
    }

    public void UpdateVisuals(bool selected)
    {
        image.sprite = Icon;        
        UpdateSelected(selected);
    }

    public void UpdateSelected(bool selected)
    {
        background.color = selected ? selectedColour : defaultColour;
    }

    public void SetText(Text text, int count)
    {
        text.text = Name.Trim() + " x" + count;
    }
}