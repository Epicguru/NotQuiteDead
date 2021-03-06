﻿using System;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.SceneManagement;

public class MainMenuRealityLoader : MonoBehaviour
{
    [Header("As Host")]
    public bool Host;
    public string RealityName;

    [Header("As Client")]
    public string IP;

    [Header("Both")]
    public int Port;

    private bool PostHost = false;

    public void Start ()
	{
        DontDestroyOnLoad(this.gameObject);

        SceneManager.activeSceneChanged += SceneChange;
	}

    public void SceneChange(Scene oldScene, Scene newScene)
    {
        if (newScene.name != "Setup V2")
            return;

        // Load reality if host...
        if (Host)
        {
            Debug.Log("Loading '" + RealityName + "' as host in '" + newScene.name + "'...");

            NetworkManager m = FindObjectOfType<NetworkManager>();
            if(m != null)
            {
                m.StopClient();
                NetworkTransport.Init();
                NetworkServer.Reset();
                m.networkPort = Port;
                m.StartHost();
                PostHost = true;
                Debug.Log("Started host on port " + Port + "...");
            }
            else
            {
                Debug.LogError("Could not find net manager, cannot start host!");
            }
        }
        else
        {
            // Connect using client...
            Debug.Log("Connecting to '" + IP + "' on port " + Port + "...");
            NetworkManager m = FindObjectOfType<NetworkManager>();
            m.networkPort = Port;
            m.networkAddress = IP;
            m.StartClient();
        }
    }

    public void Update()
    {
        if (Host && PostHost)
        {
            if(Player.Local != null)
            {
                try
                {
                    // Load world once the player has been set up.
                    World w = FindObjectOfType<World>();
                    if (w != null)
                    {
                        w.RealityName = RealityName;
                        w.Load();
                        Debug.Log("Finished loading world.");
                    }
                    else
                    {
                        Debug.LogError("World was null, cannot load reality!");
                    }
                }
                catch (Exception e)
                {
                    Debug.Log(e);
                }

                // Destroy this object, cycle done.
                Destroy(this.gameObject);
                PostHost = false;
            }
        }   
    }
}