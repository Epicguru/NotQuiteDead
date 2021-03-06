﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuRealityDetails : MonoBehaviour
{
    [Header("References")]
    public Text Title;
    public Text D_Day;
    public Text D_LastPlayed;
    public Text EnterReality;
    public Button Play;
    public Button Delete;
    public Text DeleteConfirmationName;
    public GameObject DeleteScreen;
    public MainMenuRealityItemManager Items;

    public string RealityName
    {
        get
        {
            return _RealityName;
        }
        set
        {
            _RealityName = value;
            RealityNameChange();
        }
    }
    [SerializeField]
    [ReadOnly]
    private string _RealityName;

    public void OnEnable()
    {
        DeleteScreen.SetActive(false);
    }

    public void DeleteCurrent()
    {
        if (RealityName == null)
        {
            return;
        }

        Debug.Log("Selected delete for '" + RealityName + "'");

        // Open confirmation menu.
        DeleteConfirmationName.text = "\"" + RealityName.Trim() + "\"";
        DeleteScreen.SetActive(true);
    }

    public void ConfirmDelete()
    {
        DeleteScreen.SetActive(false);

        // Delete currently selected reality.
        if(RealityName == null)
        {
            return;
        }

        // Permanently delete!
        Directory.Delete(OutputUtils.RealitySaveDirectory + RealityName, true);

        // Now refresh the whole view.
        Items.RefreshWithFolderContents();

        RealityName = null;
    }

    public void CancelDelete()
    {
        DeleteScreen.SetActive(false);
    }

    public void PlayCurrent()
    {
        if(RealityName == null)
        {
            return;
        }

        Debug.Log("Selected play for reality '" + RealityName + "'");

        // Setup cross scene loader...
        MainMenuRealityLoader loader = FindObjectOfType<MainMenuRealityLoader>();

        if(loader != null)
        {
            loader.Host = true;
            loader.RealityName = RealityName;
            // Leave port as is.

            MainMenuFunctions f = FindObjectOfType<MainMenuFunctions>();

            // Use the main menu functions class to load...
            // Looks odd because the code is ripped from that class.
            if(f != null)
            {
                f.TM.CurrentScreen = MainMenuScreen.LOADING;
                f.T.NotInMenu = true;
                f.LT.RealityName = RealityName.Trim();
                f.LT.RealityDay = 0;
                AsyncOperation ao = SceneManager.LoadSceneAsync("Setup V2", LoadSceneMode.Single);
                f.AO = ao;
            }
        }
    }

    public void RealityNameChange()
    {
        if(RealityName == null)
        {
            Title.text = "LoadReality_SelectPrompt".Translate();
            D_Day.text = "LoadReality_DayLabel".Translate("<color=white>---</color>");
            D_LastPlayed.text = "LoadReality_LastPlayed".Translate("<color=white>---</color>", "");
            EnterReality.text = "LoadReality_NoneSelected".Translate();
            Play.interactable = false;
            Delete.interactable = false;
        }
        else
        {
            Title.text = RealityName.Trim();
            D_Day.text = "LoadReality_DayLabel".Translate("<color=white>0</color>");
            D_LastPlayed.text = "LoadReality_LastPlayed".Translate("<color=white>Never</color>", "");
            EnterReality.text = "LoadReality_EnterReality".Translate(RealityName.Trim());
            Play.interactable = true;
            Delete.interactable = true;

            WorldSaveState st = null;
            try
            {
                st = WorldIO.GetSaveState(RealityName);
            }
            catch
            {
                Debug.LogWarning(string.Format("Error loading reality state for '{0}'", RealityName));
            }

            if(st != null)
            {
                D_Day.text = "LoadReality_DayLabel".Translate("<b><color=white>" + (int)(st.Time) + "</color></b>");
                D_LastPlayed.text = "LoadReality_LastPlayed".Translate("<b><color=white>" + st.LastPlayed.ToString("g") + "</color></b>", "<b><color=white>" + st.LastPlayed.TimeAgo() + "</color></b>");
            }
        }
    }
}