using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(EnemyDeath))]
[RequireComponent(typeof(NetPositionSync))]
[RequireComponent(typeof(EnemyAnimation))]
[RequireComponent(typeof(EnemyAnimationSync))]
[RequireComponent(typeof(AI))]
[RequireComponent(typeof(AIMelee))]
[RequireComponent(typeof(EnemyDirection))]
[RequireComponent(typeof(TargetDirectionProvider))]
[RequireComponent(typeof(EnemyTargetPlayer))]
[RequireComponent(typeof(EnemyAvoidBunching))]
[RequireComponent(typeof(SpriteLighting))]
[RequireComponent(typeof(Health))]
public class Enemy : NetworkBehaviour
{
    public static List<Enemy> Enemies = new List<Enemy>();

    [HideInInspector] public NetPositionSync NetPosition;
    [HideInInspector] public EnemyAnimationSync AnimationSync;
    [HideInInspector] public EnemyAvoidBunching Bunching;
    [HideInInspector] public EnemyTargetPlayer TargetPlayer;
    [HideInInspector] public AI AI;
    [HideInInspector] public AIMelee Melee;
    [HideInInspector] public EnemyAnimation Animation;
    [HideInInspector] public EnemyDeath Death;
    [HideInInspector] public EnemyDirection Direction;
    [HideInInspector] public TargetDirectionProvider TargetDirectionProvider;

    public void Start()
    {
        if(!Enemies.Contains(this))
            Enemies.Add(this);

        NetPosition = GetComponent<NetPositionSync>();
        AnimationSync = GetComponent<EnemyAnimationSync>();
        AI = GetComponent<AI>();
        Melee = GetComponent<AIMelee>();
        Bunching = GetComponent<EnemyAvoidBunching>();
        Animation = GetComponent<EnemyAnimation>();
        Death = GetComponent<EnemyDeath>();
        Direction = GetComponent<EnemyDirection>();
        TargetDirectionProvider = GetComponent<TargetDirectionProvider>();
    }

    public void OnDestroy()
    {
        if(Enemies.Contains(this))
            Enemies.Remove(this);
    }

    public void UponDeath()
    {
        if (Enemies.Contains(this))
            Enemies.Remove(this);
    }	
}
