
using UnityEngine;

public class BuildingUI : MonoBehaviour
{
    [Header("Controls")]
    public bool HeadbarOpen;
    public bool MenuOpen;

    [Header("Menu GO's")]
    public GameObject HeadbarGO; 
    public GameObject MenuGO;

    [Header("References")]
    public BuildingMenuUI Menu;

    public void Update()
    {
        if (MenuOpen)
        {
            HeadbarOpen = true;
        }

        if (MenuOpen)
        {
            Menu.Open();
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