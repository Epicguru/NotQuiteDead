using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;

public class Threader : MonoBehaviour
{
    public static Threader Instance;

    public int MaxEventsPerFrame = 100;
    public int ThreadWaitTime = 10;

    private List<UnityAction<object[]>> actions = new List<UnityAction<object[]>>();
    private List<object[]> objects = new List<object[]>();
    private bool processing = false;

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
    }

    public void PostAction(UnityAction<object[]> action, params object[] objects)
    {
        while (processing)
        {
            Thread.Sleep(ThreadWaitTime);
        }

        this.actions.Add(action);
        this.objects.Add(objects);
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
        }

        processing = false;
    }
}