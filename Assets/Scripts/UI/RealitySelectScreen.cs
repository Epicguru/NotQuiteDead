using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RealitySelectScreen : MonoBehaviour
{
    [Header("References")]
    public MainMenuTransitions T;
    public MainMenuRealityDetails Details;

    public void OnEnable()
    {
        Details.RealityName = null;
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            T.NotInMenu = false;
        }
    }
}