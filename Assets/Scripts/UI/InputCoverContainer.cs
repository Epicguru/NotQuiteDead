using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputCoverContainer : MonoBehaviour {

    public static InputCoverContainer Instance;
    public InputCover Cover;

    public void Start()
    {
        Instance = this;
    }
}
