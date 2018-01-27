
using UnityEngine;

public class BuildingUI : MonoBehaviour
{
    [Header("Controls")]
    public bool HeadbarOpen;
    public bool MenuOpen;

    [Header("Menus")]
    public GameObject HeadbarGO; 
    public GameObject MenuGO;

    public void Update()
    {
        if (MenuOpen)
        {
            HeadbarOpen = true;
        }

        HeadbarGO.SetActive(HeadbarOpen);
        MenuGO.SetActive(MenuOpen);
    }

    public void OpenMenu()
    {
        if (MenuOpen)
            return;
        MenuOpen = true;

    }

    public void CloseMenu()
    {
        if (!MenuOpen)
            return;
        MenuOpen = false;


    }
}