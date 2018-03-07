using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    public void DeleteCurrent()
    {
        if (RealityName == null)
        {
            return;
        }

        Debug.Log("Wants to delete '" + RealityName + "'");
    }

    public void PlayCurrent()
    {
        if(RealityName == null)
        {
            return;
        }

        Debug.Log("Wants to play '" + RealityName + "'");
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