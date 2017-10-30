using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class ItemOptionsPanel : MonoBehaviour
{
    public GameObject Prefab;
    public Transform Parent;
    public float Width = 250f;

    private const float START_Y = -40f;
    private const float INCREMENT = -30f;
    private List<GameObject> spawns = new List<GameObject>();
    private bool open;

    public void Update()
    {
        if (InputManager.InputDown("Return", true))
        {
            Close();
        }
    }

    public void Open(InventoryItem item)
    {
        if (open)
            return;

        ItemOption[] options = item.Item.CreateOptions(item.Item, item.Data);
        SetOptions(options, item);
    }

    public void Open(GearItem item)
    {
        if (open)
            return;

        item.Item.RequestDataUpdate(); // Get new data.
        ItemOption[] options = item.GetOptions(item.Item.Data);
        SetOptions(options, new InventoryItem() { ItemPrefab = item.Item.Prefab, ItemCount = 1, Inventory = PlayerInventory.inv.Inventory, Resize = false, Item = Item.FindItem(item.Item.Prefab), Data = item.Item.Data});
    }

    private void SetOptions(ItemOption[] options, InventoryItem item)
    {
        open = true;

        GetComponentInChildren<Text>().text = item.Item.Name + "\nOptions";

        int index = 0;
        foreach (ItemOption option in options)
        {
            float y = START_Y + INCREMENT * index;

            GameObject GO = Instantiate(Prefab, Parent);
            (GO.transform as RectTransform).anchoredPosition = new Vector2(0, y);

            GO.GetComponentInChildren<Text>().text = option.OptionName;

            option.InvItem = item;
            GO.GetComponent<Button>().onClick.AddListener(option.Clicked);
            GO.GetComponent<Button>().onClick.AddListener(Close);

            spawns.Add(GO);

            index++;
        }

        (Parent.GetComponent<Transform>() as RectTransform).anchoredPosition = Input.mousePosition;
        float height = (START_Y + INCREMENT * options.Length) * -1f + 2.5f;
        (Parent.GetComponent<Transform>() as RectTransform).sizeDelta = new Vector2(Width, height);
        Vector2 pos = (Parent.GetComponent<Transform>() as RectTransform).anchoredPosition;
        if (Input.mousePosition.x + Width > Screen.width)
            pos.x = Screen.width - Width;
        if (Input.mousePosition.y - height < 0)
            pos.y = height;

        (Parent.GetComponent<Transform>() as RectTransform).anchoredPosition = pos;

        gameObject.SetActive(true);
    }

    public void Close()
    {
        if (!open)
            return;

        foreach(GameObject GO in spawns)
        {
            Destroy(GO);
        }
        spawns.Clear();

        gameObject.SetActive(false);

        open = false;
    }
}
