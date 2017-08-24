using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class InputManagerSetup : MonoBehaviour
{
    public void Awake()
    {
        InputManager.LoadKeyBindings();
    }

    public void FixedUpdate()
    {
        InputManager.UpdateMousePos();
    }
}
