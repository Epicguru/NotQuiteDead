
using UnityEngine;

public class BuildingUI : MonoBehaviour
{
    [Header("Controls")]
    public bool BarOpen;
    public bool MenuOpen;

    [Header("Menu GO's")]
    public GameObject HeadbarGO; 
    public GameObject MenuGO;

    [Header("References")]
    public BuildingMenuUI Menu;
    public BuildingBarUI Bar;

    public static BuildingUI Instance;

    public void Update()
    {
        if (UI.AnyOpen)
        {
            // Is that UI the menu?
            if (Menu.IsOpen)
            {
                // That's fine!
            }
            else
            {
                // Our menus cannot be open at the same time as another UI.
                MenuOpen = false;
                BarOpen = false;
            }
        }

        if (MenuOpen)
        {
            BarOpen = true;
        }

        HeadbarGO.SetActive(BarOpen);
        MenuGO.SetActive(MenuOpen);

        if (MenuOpen)
        {
            Menu.Open();
        }
        else
        {
            Menu.Close();
        }

        if (BarOpen)
        {
            Bar.Open();
        }
        else
        {
            Bar.Close();
        }
    }

    public void Awake()
    {
        Instance = this;
    }

    public void OnDestroy()
    {
        Instance = null;
    }
}