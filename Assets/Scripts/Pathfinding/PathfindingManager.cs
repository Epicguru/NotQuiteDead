﻿
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;

public static class PathfindingManager
{
    public const int MAX_PENDING = 500;
    private static Queue<PathfindingRequest> pending = new Queue<PathfindingRequest>();
    private static List<long> times = new List<long>();
    private static Thread thread;
    private static bool writing;
    private static bool run;
    private static Stopwatch watch = new Stopwatch();
    private static HashSet<string> hasPending = new HashSet<string>();
    private static List<KeyValuePair<UnityAction<List<Node>>, List<Node>>> finished = new List<KeyValuePair<UnityAction<List<Node>>, List<Node>>>();
    private static bool reading;
    private static int acc;
    private static int solved;
    private static float timer;

    private static void Run()
    {
        // Runs on the thread.
        UnityEngine.Debug.Log("Running pathfinding multithread...");
        while (run)
        {
            if (!writing)
            {
                if (pending.Count == 0)
                {
                    Thread.Sleep(5);
                    continue;
                }

                PathfindingRequest request = pending.Dequeue();
                if (request.IsValid())
                {
                    watch.Reset();
                    watch.Start();

                    // Pathfind here.
                    List<Node> path = Pathfinding.Run(request.StartX, request.StartY, request.EndX, request.EndY, request.Layer);

                    acc++;

                    watch.Stop();
                    long elapsed = watch.ElapsedMilliseconds;
                    times.Add(elapsed);

                    if (times.Count > 100)
                    {
                        times.RemoveAt(0);
                    }

                    if (request.Done != null)
                    {
                        lock (hasPending)
                        {
                            hasPending.Remove(request.ID);
                        }
                        while (reading)
                        {
                            Thread.Sleep(10);
                        }
                        finished.Add(new KeyValuePair<UnityAction<List<Node>>, List<Node>>(request.Done, path));
                    }
                    else
                    {
                        lock (hasPending)
                        {
                            hasPending.Remove(request.ID);
                        }
                        UnityEngine.Debug.LogWarning("Wasted pathfinding (" + (path == null ? "NO PATH" : "PATH FOUND") + "), no receptor!");
                    }
                }
            }
        }
        UnityEngine.Debug.Log("Shutdown pathfinding multithread.");
    }

    public static bool Find(string ID, int x, int y, int ex, int ey, TileLayer layer, UnityAction<List<Node>> done)
    {
        if (!run)
        {
            if (done != null)
                done.Invoke(null);
            return false;
        }

        if(pending.Count >= MAX_PENDING)
        {
            if(done != null)
                done.Invoke(null);
            return false;
        }

        if (hasPending.Contains(ID))
        {
            return false;
        }

        PathfindingRequest r = new PathfindingRequest() { StartX = x, StartY = y, EndX = ex, EndY = ey, Layer = layer, Done = done, ID = ID };

        if (r.IsValid())
        {
            writing = true;
            lock (hasPending)
            {
                hasPending.Add(ID);
            }
            pending.Enqueue(r);
            writing = false;
            return true;
        }
        else
        {
            if(done != null)
                done.Invoke(null);
            return false;
        }
    }

    public static void Update()
    {
        ProcessDone();

        timer += Time.unscaledDeltaTime;
        if(timer >= 1f)
        {
            timer -= 1f;
            solved = acc;
            acc = 0;
        }

        try
        {
            DebugText.Log(pending.Count + " / " + MAX_PENDING + " pathfinding operations pending, " + hasPending.Count + " unique keys.", Color.Lerp(Color.green, Color.red, pending.Count / (float)MAX_PENDING));
            if (times.Count > 0)
            {
                DebugText.Log("Solved per second: " + solved + " <--", Color.yellow);
                DebugText.Log("Latest path time: " + times[times.Count - 1] + "ms <--", Color.yellow);
                DebugText.Log("Average path time (" + times.Count + " samples): " + Mathf.RoundToInt((float)times.Average()) + "ms <--", Color.yellow);
                DebugText.Log("Lowest path time (" + times.Count + " samples): " + times.Min() + "ms <--", Color.yellow);
                DebugText.Log("Highest path time (" + times.Count + " samples): " + times.Max() + "ms <--", Color.yellow);
                DebugText.Log(finished.Count + " processed requests pending for application.", Color.yellow);
            }
        }
        catch
        {
            // Gotta catch em' all!
        }
    }

    private static void ProcessDone()
    {
        reading = true;

        foreach(var x in finished)
        {
            x.Key.Invoke(x.Value);
        }
        finished.Clear();

        reading = false;
    }

    public static HashSet<string> GetPending()
    {
        return hasPending;
    }

    public static void DissolveAllPending()
    {
        foreach(var x in pending)
        {
            if (x.Done != null)
                x.Done.Invoke(null);
        }
        pending.Clear();
    }

    public static string GetPendingIDs()
    {
        return pending.ToString();
    }

    public static void Stop()
    {
        if (!run)
            return;

        run = false;
        writing = true;
        pending.Clear();
        writing = false;
    }

    public static void Start()
    {
        if (run == true)
            return;

        run = true;
        writing = false;
        reading = false;
        if (pending == null)
            pending = new Queue<PathfindingRequest>();
        pending.Clear();

        thread = new Thread(() =>
        {
            Run();
        });

        thread.Start();
    }
}