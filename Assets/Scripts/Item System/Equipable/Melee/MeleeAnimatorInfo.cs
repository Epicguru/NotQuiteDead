using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[System.Serializable]
public class MeleeAnimatorInfo
{
    [Tooltip("The Animator for this weapon.")]
    public Animator Animator;
    [Tooltip("The minimum value to assign to the random animation variable.")]
    public int MinRandom = 0;
    [Tooltip("The maximum value to assign to the random animation variable.")]
    public int MaxRandom = 1;
    [Tooltip("The attack trigger name, in animation.")]
    public string Attack = "Attack";
    [Tooltip("The random integer name, in animation.")]
    public string Random = "Random";
    [Tooltip("The equip trigger name, in animation.")]
    public string Equip = "Equip";
    [Tooltip("The dropped bool name, in animation.")]
    public string Dropped = "Dropped";
    [Tooltip("If true, the first attack (combo 0) will always have a random value of 0.")]
    public bool FirstAttackIsNotRandom = false;
    [Tooltip("Requires that a new random animation is selected each attack, if there is more than one.")]
    public bool RequireNewRandom = true;
}
