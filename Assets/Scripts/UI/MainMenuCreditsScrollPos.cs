using UnityEngine;
using UnityEngine.UI;

public class MainMenuCreditsScrollPos : MonoBehaviour
{
    public ScrollRect SV;

    public void OnEnable()
    {
        SV.verticalNormalizedPosition = 1f;
    }
}
