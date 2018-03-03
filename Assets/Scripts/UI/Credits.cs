using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Credits : MonoBehaviour
{
    public ScrollRect SV;
    public Text Text;

    public void Start()
    {
        SetText();
    }

    public void SetText()
    {
        Text.text = Resources.Load<TextAsset>("Credits").text;
    }

    public void Update()
    {
        SV.verticalNormalizedPosition -= 0.1f * Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("Main Menu");
        }
    }
}