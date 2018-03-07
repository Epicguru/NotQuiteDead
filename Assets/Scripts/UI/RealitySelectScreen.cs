using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RealitySelectScreen : MonoBehaviour
{
    public MainMenuTransitions T;

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            T.NotInMenu = false;
        }
    }
}