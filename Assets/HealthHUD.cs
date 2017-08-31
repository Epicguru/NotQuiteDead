using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthHUD : MonoBehaviour {

    public Text Text;

    public void Update()
    {
        if(Player.Local != null)
        {
            Text.text = Player.Local.Health.GetHealth().ToString();
        }
    }
}
