using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class ChunkRegenerator : MonoBehaviour
{
    // Used to limit the amount of time spent on regenrating chunk backgrounds per frame.

    public static ChunkRegenerator Instance;

    public int MaxTimePerFrame = 2;
    private Queue<ChunkBackground> backgrounds = new Queue<ChunkBackground>();
    private Stopwatch timer = new Stopwatch();

    public void Regenerate(ChunkBackground bg)
    {
        if(!backgrounds.Contains(bg))
            backgrounds.Enqueue(bg);
    }

    public void Update()
    {
        GenerateBackgrounds(MaxTimePerFrame);
    }

    public void GenerateBackgrounds(int maxTime)
    {
        timer.Start();

        bool run = true;
        while (run)
        {
            var bg = backgrounds.Dequeue();
            if (bg == null)
                continue;

            bg.Regenerate();

            if (timer.ElapsedMilliseconds >= maxTime)
            {
                run = false;
            }
        }
    }

    public void Awake()
    {
        Instance = this;
    }

    public void OnDestroy()
    {
        Instance = null;
    }
}