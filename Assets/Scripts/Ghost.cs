using UnityEngine;
using System.Collections;

public class Ghost : MonoBehaviour
{
    public float TimeAlive;
    private float timer;
    private new SpriteRenderer renderer;
    private static Color colour = new Color();

    public void Start()
    {
        renderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        float p = (TimeAlive - timer) / TimeAlive;

        colour.r = renderer.color.r;
        colour.g = renderer.color.g;
        colour.b = renderer.color.b;
        colour.a = p;
        renderer.color = colour;

        if(timer >= TimeAlive)
        {
            Destroy(this.gameObject);
        }

        transform.Translate(0, -2.5f * Time.deltaTime, 0);
    }
}
