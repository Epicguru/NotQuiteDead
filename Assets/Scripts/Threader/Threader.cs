using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Has methods for calling unity actions from different threads that will execute in the main unity thread.
/// Only works for UnityActions that take an array of objects as a paramter.
/// When specifiying action params from methods, use the following convention:
/// public void Foo (string param, int param2, UnityAction<object[]> actionName_paramType1_paramType2)
/// </summary>
public class Threader : MonoBehaviour
{
    public static Threader Instance;

    [ReadOnly]
    public int PendingOperations;
    [ReadOnly]
    public int ExecutedOperationsOver10Seconds;
    [ReadOnly]
    public int SleepTimeOver10Seconds;
    [Range(0f, 120f)]
    public float UpdatesPerSecond = 60f;
    public int MaxActionsPerUpdate = 100;
    [Range(0, 1000)]
    public int ThreadWaitTime = 10;
    public bool AllowFrameStacking = false;

    private List<UnityAction<object[]>> actions = new List<UnityAction<object[]>>();
    private List<object[]> objects = new List<object[]>();
    private bool processing = false;
    private float timer;
    private float statsTimer;
    private System.Object myLock = new System.Object();

    public void Awake()
    {
        Instance = this;
        PathfindingManager.Start();
    }

    public void OnDestroy()
    {
        Instance = null;
        PathfindingManager.Stop();
        Pathfinding.Clean();
    }

    public void Update()
    {
        float deltaTime = Time.unscaledDeltaTime;

        PathfindingManager.Update();

        UpdateTimer(deltaTime);
        UpdateStats(deltaTime);
    }

    public void UpdateStats(float deltaTime)
    {
        statsTimer += deltaTime;
        if (statsTimer >= 10f)
        {
            statsTimer -= 10f;
            SleepTimeOver10Seconds = 0;
            ExecutedOperationsOver10Seconds = 0;
        }

        DebugText.Log(RichText.InColour("Threader sleep time: " + ThreadWaitTime, Color.grey));
        DebugText.Log(RichText.InColour("Threader updates per second: " + UpdatesPerSecond, Color.grey));
        DebugText.Log(RichText.InColour("Thread Stats Over 10 Seconds:", Color.cyan));
        DebugText.Log(RichText.InColour("Executed operations: " + ExecutedOperationsOver10Seconds, Color.cyan));
        DebugText.Log(RichText.InColour("Time spent sleeping: " + SleepTimeOver10Seconds, Color.cyan));
    }

    public void UpdateTimer(float deltaTime)
    {
        timer += deltaTime;
        float interval = GetInterval();

        while(timer >= interval)
        {
            timer -= interval;
            ProcessEvents(MaxActionsPerUpdate);

            if (!AllowFrameStacking)
                break;
        }
    }

    public float GetInterval()
    {
        return 1f / UpdatesPerSecond;
    }

    public void PostAction(UnityAction<object[]> action, params object[] objects)
    {
        lock (myLock)
        {
            if (action == null)
                return;

            while (processing)
            {
                Thread.Sleep(ThreadWaitTime);
                SleepTimeOver10Seconds += ThreadWaitTime;
            }

            this.actions.Add(action);
            this.objects.Add(objects);
            PendingOperations++;
        }        
    }

    public void ProcessEvents(int max)
    {
        processing = true;
        for (int i = 0; i < max; i++)
        {
            if (actions.Count == 0)
                break;

            UnityAction<object[]> action = actions[0];

            object[] objs = objects[0];

            action.Invoke(objs);

            actions.RemoveAt(0);
            objects.RemoveAt(0);

            ExecutedOperationsOver10Seconds++;

            PendingOperations--;
            if (PendingOperations < 0)
                PendingOperations = 0;
        }

        processing = false;
    }
}