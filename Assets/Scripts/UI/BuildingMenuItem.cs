using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BuildingMenuItem : MonoBehaviour
{
    [Header("Code Only")]
    public Color BGColour;
    public string Name;
    public int Count;
    public Sprite Icon;

    [Header("Ghost")]
    public GameObject GhostPrefab;
    public GameObject Ghost;
    [ReadOnly]
    public bool Dragging;

    [Header("Editor Only")]
    [SerializeField]
    private Text text;
    [SerializeField]
    private Image icon;
    [SerializeField]
    private Image BG;

    public void Start()
    {
        EventTrigger trigger = GetComponent<EventTrigger>();
        EventTrigger.Entry entry;

        // Drag begin.
        entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.BeginDrag;
        entry.callback.AddListener((data) => { DragBegin((PointerEventData)data); });
        trigger.triggers.Add(entry);

        // On Drag.
        entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.Drag;
        entry.callback.AddListener((data) => { DragOngoing((PointerEventData)data); });
        trigger.triggers.Add(entry);

        // End Drag.
        entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.EndDrag;
        entry.callback.AddListener((data) => { DragEnd((PointerEventData)data); });
        trigger.triggers.Add(entry);
    }

    public void UpdateVisuals()
    {
        text.text = Name.Trim() + " x" + Count;
        icon.sprite = Icon;
        BGColour.a = 0.7f;
        BG.color = BGColour;
    }

    public void DragBegin(PointerEventData eventData)
    {
        if (Dragging)
            return;

        Dragging = true;

        // Create the ghost object and parent to this.
        GameObject go = Instantiate(GhostPrefab, GetComponentInParent<BuildingMenuUI>().GhostParent);
        Ghost = go;
    }

    public void DragOngoing(PointerEventData eventData)
    {
        if (!Dragging)
            return;

        if (Ghost == null)
        {
            Dragging = false;
            return;
        }

        // Update the ghost's position.
        Ghost.transform.position = eventData.position;
    }

    public void DragEnd(PointerEventData eventData)
    {
        if (!Dragging)
            return;

        Dragging = false;

        // Destroy the ghost
        if (Ghost != null)
        {
            Destroy(Ghost);
        }
    }
}