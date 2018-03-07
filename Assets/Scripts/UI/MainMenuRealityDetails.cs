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

        SceneManager.LoadScene("Setup V2");
    }

    public void RealityNameChange()
    {
        if(RealityName == null)
        {
            Title.text = "Select a reality...";
            D_Day.text = "Day: <color=white>---</color>";
            D_LastPlayed.text = "Last Played: <color=white>-/-/-</color>";
            EnterReality.text = "No Reality Selected";
            Play.interactable = false;
            Delete.interactable = false;
        }
        else
        {
            Title.text = RealityName.Trim();
            D_Day.text = "Day: <color=white>" + "TODO" + "</color>";
            D_LastPlayed.text = "Last Played: <color=white>" + "x/y/z" + "</color>";
            EnterReality.text = "Enter " + RealityName.Trim();
            Play.interactable = true;
            Delete.interactable = true;
        }
    }
}