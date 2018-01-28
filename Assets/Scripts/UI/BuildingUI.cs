
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

    public void Update()
    {
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
}