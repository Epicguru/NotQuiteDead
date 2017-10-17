using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class EnemyDeath : NetworkBehaviour
{
    public float TimeAsCorpse = 10f;
    public float FadeoutTime = 5f;
    public bool Dead = false;
    [HideInInspector] public Health Health;
    [HideInInspector] public EnemyAnimation Animation;
    [HideInInspector] public EnemyTargetPlayer Targeting;
    [HideInInspector] public AI AI;
    [HideInInspector] public EnemyAnimationSync AnimationSync;

    private float timer;
    private SpriteRenderer[] renderers;
    private Color colour = new Color();

    public void Start()
    {
        Targeting = GetComponent<EnemyTargetPlayer>();
        Health = GetComponent<Health>();
        Animation = GetComponent<EnemyAnimation>();
        AI = GetComponent<AI>();
        AnimationSync = GetComponent<EnemyAnimationSync>();
        Health.UponDeath += UponDeath;
    }

    public void Update()
    {
        if (Dead)
        {
            Animation.Dead = true;
            Targeting.Active = false;
            AI.Active = false;
            AnimationSync.Active = false;
            timer = Mathf.Clamp(Time.deltaTime + timer, 0f, TimeAsCorpse + FadeoutTime);
            if(timer >= TimeAsCorpse)
            {
                // Start to fade.
                float alpha = Mathf.Lerp(1f, 0f, (timer - TimeAsCorpse) / FadeoutTime);

                if (renderers == null)
                    renderers = GetComponentsInChildren<SpriteRenderer>();

                foreach(SpriteRenderer r in renderers)
                {
                    if (r == null)
                        return;
                    if (r.GetComponent<Arrow>() != null)
                        continue;
                    colour.r = r.color.r;
                    colour.g = r.color.g;
                    colour.b = r.color.b;
                    colour.a = alpha;

                    r.color = colour;
                }
            }
        }
    }

    public virtual void UponDeath()
    {
        Dead = true;
        foreach(Collider2D c in GetComponentsInChildren<Collider2D>())
        {
            if(!Health.CannotHit.Contains(c))
            {
                Health.CannotHit.Add(c);
            }
        }
    }
}
