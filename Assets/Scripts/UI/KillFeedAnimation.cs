using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class KillFeedAnimation : MonoBehaviour {

    public bool Enabled = true;
    public bool Closed = false;
    public float TimeEnabled = 0f;
    public float EntryTime = 1f;

    private KillFeedEnter E;
    private float timer;

    public void Update()
    {
        if (E == null)
            E = GetComponent<KillFeedEnter>();

        timer = Mathf.Clamp(timer + Time.deltaTime * (Enabled ? 1f : -1f), 0f, EntryTime);
        float p = timer / EntryTime;

        E.Percentage = p;

        Closed = !Enabled && timer == 0;

        if(Enabled && timer == EntryTime)
        {
            TimeEnabled += Time.deltaTime;
        }
        else
        {
            TimeEnabled = 0f;
        }
    }
}
