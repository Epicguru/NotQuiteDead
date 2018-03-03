using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Credits : MonoBehaviour
{
    public RectTransform Canvas;
    public ScrollRect SV;
    public Text Text;

    public Vector2 VerticalRotation;
    public Vector2 HorizontalRotation;

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

        float x = Input.mousePosition.x / Screen.width;
        float y = Input.mousePosition.y / Screen.height;

        float rotY = Mathf.Lerp(HorizontalRotation.x, HorizontalRotation.y, x);
        float rotX = Mathf.Lerp(VerticalRotation.x, VerticalRotation.y, y);

        Canvas.rotation = Quaternion.Euler(new Vector3(rotX, rotY, 0f));
    }
}