using UnityEngine;
using System.Collections;

public class Ghost : MonoBehaviour
{
    public float TimeAlive;
    public Transform TurnObject;
    public float Turn;
    private float timer;
    private new SpriteRenderer renderer;
    private static Color colour = new Color();

    public void Start()
    {
        renderer = GetComponentInChildren<SpriteRenderer>();
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

        if(TurnObject != null)
            TurnObject.RotateAround(transform.position, transform.forward, Turn * Time.deltaTime * (renderer.flipX ? -1 : 1));
    }
}
