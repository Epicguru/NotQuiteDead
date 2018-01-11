using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Health), typeof(ActiveObject), typeof(NetPositionSync))]
public class Pawn : NetworkBehaviour
{
    // Represents something that can move around the world map, using AI and pathfinding.

    public string Prefab;
    public string Name;

    [HideInInspector]
    public Health Health;

    [HideInInspector]
    public ActiveObject AO;

    [HideInInspector]
    public NetPositionSync NetPositionSync;

    private List<Node> path;
    private float timer;

    // What should a pawn do in an unloaded chunk?
    // Lets just load the chunks around pawns!

    public void Awake()
    {
        Health = GetComponent<Health>();
        AO = GetComponent<ActiveObject>();
        NetPositionSync = GetComponent<NetPositionSync>();
    }

    public void Start()
    {
        Name = Random.Range(0, 1000000).ToString();
        Pathfinding.Find(Name + "(" + Prefab + ")", (int)transform.position.x, (int)transform.position.y, (int)Random.Range(transform.position.x - 10, transform.position.x + 10), (int)Random.Range(transform.position.y - 10, transform.position.y + 10), World.Instance.TileMap.GetLayer("Foreground"), PathFound);
    }

    public void Update()
    {
        if (!isServer)
            return;

        timer += Time.deltaTime * 15f; // * Speed

        if (path == null)
        {
            Pathfinding.Find(Name + "(" + Prefab + ")", (int)transform.position.x, (int)transform.position.y, (int)Random.Range(transform.position.x - 10, transform.position.x + 10), (int)Random.Range(transform.position.y - 10, transform.position.y + 10), World.Instance.TileMap.GetLayer("Foreground"), PathFound);
            timer = 0;
            return;
        }

        // Move along the path
        float p = Mathf.Clamp(timer / path.Count, 0, 1);

        int index = (int)(p * path.Count);

        if (index >= path.Count)
            index = path.Count - 1;

        float remainder = (p * path.Count) - index;

        Node start = path[index];
        Node end = path[index + 1 == path.Count ? index : index + 1];
        Vector3 position = Vector3.Lerp(new Vector3(start.X+  0.5f, start.Y + 0.5f), new Vector3(end.X + 0.5f, end.Y + 0.5f), remainder);

        transform.position = position;

        if(p == 1)
        {
            path = null;            
        }
    }

    private void PathFound(List<Node> path)
    {
        this.path = path;
    }

    public void OnDrawGizmosSelected()
    {
        Pathfinding.DrawPathLine(path, Color.magenta);
    }
}