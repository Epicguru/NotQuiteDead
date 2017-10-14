using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Workbench : MonoBehaviour
{
    public BlueprintPreviewDisplay Preview;
    public Transform ItemParent;

    public List<Blueprint> Blueprints = new List<Blueprint>();

    public void Awake()
    {
        
    }
}
