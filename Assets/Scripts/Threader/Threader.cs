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

    public int PendingOperations;
    public int ExecutedOperations10Seconds;
    public int SleepTime10Seconds;

    public int MaxEventsPerFrame = 100;
    public int ThreadWaitTime = 10;

    private List<UnityAction<object[]>> actions = new List<UnityAction<object[]>>();
    private List<object[]> objects = new List<object[]>();
    private bool processing = false;
    private float timer;

    public void Awake()
    {
        Instance = this;
    }

    public void OnDestroy()
    {
        Instance = null;
    }

    public void Update()
    {
        ProcessEvents(MaxEventsPerFrame);

        timer += Time.unscaledDeltaTime;
        if(timer >= 10f)
        {
            timer -= 10f;
            SleepTime10Seconds = 0;
            ExecutedOperations10Seconds = 0;
        }
    }

    public void PostAction(UnityAction<object[]> action, params object[] objects)
    {
        if (action == null)
            return;

        while (processing)
        {
            Thread.Sleep(ThreadWaitTime);
            SleepTime10Seconds += ThreadWaitTime;
        }

        this.actions.Add(action);
        this.objects.Add(objects);
        PendingOperations++;
    }

    public void ProcessEvents(int max)
    {
        processing = true;
        for (int i = 0; i < max; i++)
        {
            if (actions.Count == 0)
                break;

            UnityAction<object[]> action = actions[0];
            actions.RemoveAt(0);
            object[] objs = objects[0];
            objects.RemoveAt(0);

            action.Invoke(objs);
            PendingOperations--;
            ExecutedOperations10Seconds++;
            if (PendingOperations < 0)
                PendingOperations = 0;
        }

        processing = false;
    }
}