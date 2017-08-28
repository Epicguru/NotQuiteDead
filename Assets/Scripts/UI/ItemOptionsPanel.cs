﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class ItemOptionsPanel : MonoBehaviour
{
    public GameObject Prefab;
    public Transform Parent;

    private const float START_Y = -40f;
    private const float INCREMENT = -30f;
    private List<GameObject> spawns = new List<GameObject>();
    private Item last;

    public void Update()
    {
        if (InputManager.InputDown("Return"))
        {
            Close();
        }
    }

    public void Open(InventoryItem item)
    {
        if (last != null)
            return;

        ItemOption[] options = item.Item.CreateOptions();

        //Debug.Log(item.Item.Name + " has " + options.Length + " options.");

        last = item.Item;

        GetComponentInChildren<Text>().text = item.Item.Name + "\nOptions";

        int index = 0;
        foreach(ItemOption option in options)
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
        float height = START_Y + INCREMENT * options.Length * -1f;
        (Parent.GetComponent<Transform>() as RectTransform).sizeDelta = new Vector2(120, height);
        Vector2 pos = (Parent.GetComponent<Transform>() as RectTransform).anchoredPosition;
        if (Input.mousePosition.x + 120 > Screen.width)
            pos.x = Screen.width - 120;
        if (Input.mousePosition.y - height < 0)
            pos.y = height;

        (Parent.GetComponent<Transform>() as RectTransform).anchoredPosition = pos;

        gameObject.SetActive(true);
    }

    public void Close()
    {
        if (last == null)
            return;

        foreach(GameObject GO in spawns)
        {
            Destroy(GO);
        }
        spawns.Clear();

        gameObject.SetActive(false);

        last = null;
    }
}
