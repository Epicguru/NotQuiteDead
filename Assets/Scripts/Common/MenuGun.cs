using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MenuGun : MonoBehaviour {

	public void RemoveAllComponents()
    {
        Debug.Log("Removing components.");
        foreach(MonoBehaviour c in GetComponentsInChildren<MonoBehaviour>())
        {
            if(c != this && c.GetType() != typeof(Hand))
                c.enabled = false;
        }
        GetComponent<Rigidbody2D>().simulated = false;
        GetComponentInChildren<ItemAnimationCallback>().Active = false;
    }

    public void Awake()
    {
        Debug.Log("Something!");
        RemoveAllComponents();
        InvokeRepeating("MyUpdate", 0f, 0.05f);
    }

    public void MyUpdate()
    {
        gameObject.SetActive(true);        
    }
}
