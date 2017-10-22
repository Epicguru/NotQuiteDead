using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MiningDrill : NetworkBehaviour
{
    [SyncVar]
    public bool Active;
    public Animator Animator;
    public Vector2 DropOffest;

    public List<ItemProductionRate> Production = new List<ItemProductionRate>();

    private ItemPickup pickup;
    private Placeable placeable;
    private float[] timers;

    public void Start()
    {
        pickup = GetComponent<ItemPickup>();
        placeable = GetComponent<Placeable>();
    }

    public void Update()
    {
        UpdateActiveState();
        UpdateAnimation();

        if (isServer && placeable.IsPlaced && Active)
            ProduceMaterials();
    }

    public void UpdateActiveState()
    {
        if (pickup.MouseOver && InputManager.Active)
        {
            ActionHUD.DisplayAction("Press " + InputManager.GetInput("Interact") + " to " + (Active ? "turn off" : "turn on") + " mining drill.");
            if (InputManager.InputDown("Interact"))
            {
                Player.Local.NetUtils.CmdToggleMiningDrill(gameObject, !Active);
            }
        }
    }

    public void UpdateAnimation()
    {
        Animator.SetBool("Active", Active);
    }

    [Server]
    public void ProduceMaterials()
    {
        if(timers == null || timers.Length != Production.Count)
        {
            timers = new float[Production.Count];
        }

        for (int i = 0; i < timers.Length; i++)
        {
            timers[i] += Time.deltaTime;

            float timer = timers[i];
            float interval = Production[i].Interval;

            while (timer >= interval)
            {
                timer -= interval;
                timers[i] -= interval;
                DropItem(Production[i].Prefab, Production[i].Count);
            }
        }
    }

    [Server]
    public void DropItem(string prefab, int count)
    {
        Vector2 position = (Vector2)transform.position + DropOffest;

        for (int i = 0; i < count; i++)
        {
            Player.Local.NetUtils.CmdSpawnDroppedItem(prefab, position + Random.insideUnitCircle * 0.5f, new ItemData());
        }
    }
}

[System.Serializable]
public struct ItemProductionRate
{
    public string Prefab;
    public float Interval;
    public int Count;
}