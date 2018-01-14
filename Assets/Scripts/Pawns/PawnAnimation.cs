
using UnityEngine;
using UnityEngine.Networking;

public class PawnAnimation : NetworkBehaviour
{
    [Tooltip("Should the pawn change direction based on pathfinding?")]
    public bool FacePathfinding = true;

    [SyncVar]
    public bool Right;
    [SyncVar]
    public bool Moving;
    [SyncVar]
    public bool Dead;
    [SyncVar]
    public bool Attacking;

    public Animator Anim;
    [HideInInspector]
    public Pawn Pawn;

    private bool[] paramStatus;

    public void Awake()
    {
        Pawn = GetComponent<Pawn>();

        if(Anim == null)
        {
            Anim = GetComponentInChildren<Animator>();
        }

        SetParamStatus();
    }

    public void Update()
    {
        if (isServer)
        {
            if (FacePathfinding)
            {
                bool right = Pawn.Path.MovingRight;
                if(right != Right)
                {
                    Right = right;
                }
            }

            bool dead = Pawn.Health.GetHealth() <= 0f;
            if(dead != Dead)
                Dead = dead;

            bool moving = Pawn.Path.Moving;
            if(moving != Moving)
                Moving = Pawn.Path.Moving;
        }

        if (Right)
        {
            Vector3 scale = transform.localScale;
            if (scale.x < 0)
                scale.x *= -1;
            transform.localScale = scale;
        }
        else
        {
            Vector3 scale = transform.localScale;
            if (scale.x > 0)
                scale.x *= -1;
            transform.localScale = scale;
        }

        ApplyValues();
    }

    private void SetParamStatus()
    {
        if (Anim == null)
            return;

        paramStatus = ContainsParams("Moving", "Dead", "Attack");
    }

    private bool[] ContainsParams(params string[] names)
    {
        bool[] res = new bool[name.Length];
        int i = 0;
        foreach(string name in names)
        {
            res[i] = false;
            foreach(var x in Anim.parameters)
            {
                if (x.name == name)
                {
                    res[i] = true;
                }
            }
            i++;
        }

        return res;
    }

    public void ApplyValues()
    {
        if (Anim == null)
            return;

        // Moving
        if (paramStatus[0])
        {
            Anim.SetBool("Moving", Moving);
        }

        // Dead
        if (paramStatus[1])
        {
            Anim.SetBool("Dead", Dead);
        }

        // Attacking
        if (paramStatus[2])
        {
            Anim.SetBool("Attack", Attacking);
        }
    }
}