using UnityEngine;
using System.Collections;

public class SpeedMeter : MonoBehaviour
{

    private Vector3 oldPos = new Vector3();
    public float Speed;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float distance = Vector3.Distance(transform.position, oldPos);

        oldPos = transform.position;

        Speed = distance / Time.deltaTime;
    }

    public void OnGUI()
    {
        GUI.Label(new Rect(0, 0, 300, 100), gameObject.name + " : " + string.Format("{0:0}", Speed) + " units/second.");
    }
}
