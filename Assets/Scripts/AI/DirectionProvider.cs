using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[RequireComponent(typeof(AI))]
public class DirectionProvider : MonoBehaviour
{
    public Vector2 Direction;
    [Range(0f, 1f)]
    public float Weight;

    public bool Active = true;

    [HideInInspector]
    public AI AI;

    public virtual void Start()
    {
        AI = GetComponent<AI>();
    }

    public virtual void Update()
    {
        if(Active)
            AI.Add(Direction, Weight);
    }
}
