using UnityEngine;

public class MainMenuImageCallback : MonoBehaviour
{
    public MainMenuImage Img;

    public void UpdateSprite()
    {
        Img.SetNewSprite();
    }
}