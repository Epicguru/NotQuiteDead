
using UnityEngine;

public class Disabler : MonoBehaviour
{
    public GameObject[] Subjects;
    public bool Enabled = true;

    private bool _enabled;

    public void Awake()
    {
        _enabled = Enabled;
        Apply();
    }

    public void Update()
    {
        if(_enabled != Enabled)
        {
            Apply();
        }
    }

    private void Apply()
    {
        foreach(var b in Subjects)
        {
            if (b == null)
                continue;

            b.SetActive(Enabled);
        }

        _enabled = Enabled;
    }
}