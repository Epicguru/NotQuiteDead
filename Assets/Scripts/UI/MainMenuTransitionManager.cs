using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MainMenuTransitions))]
public class MainMenuTransitionManager : MonoBehaviour
{
    public MainMenuScreen CurrentScreen;
    public RectTransform[] Screens;

    private MainMenuTransitions trans;

    public void Start()
    {
        trans = GetComponent<MainMenuTransitions>();
    }

    public void Update()
    {
        int index = (int)CurrentScreen;

        RectTransform current = Screens[index];

        for (int i = 0; i < Screens.Length; i++)
        {
            bool active = i == index;
            if (active)
            {
                if (trans.TimerAtZero())
                {
                    active = false;
                }
            }
            Screens[i].gameObject.SetActive(active);
        }

        trans.Other = current;
    }
}

public enum MainMenuScreen : int
{
    LOADING,
    REALITY_SELECT,
    CREDITS
}