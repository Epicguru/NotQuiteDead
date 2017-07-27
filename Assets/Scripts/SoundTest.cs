using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundTest : MonoBehaviour {

    private new AudioSource audio;

    public void Start()
    {
        this.audio = GetComponent<AudioSource>();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            this.audio.Play();
        }
    }
}