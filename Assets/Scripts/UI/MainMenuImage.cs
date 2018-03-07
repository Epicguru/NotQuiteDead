using UnityEngine;
using UnityEngine.U2D;

public class MainMenuImage : MonoBehaviour
{
    public Vector3 RandomRot;
    public Vector2 Interval;
    public float Magnitude;
    public float ChangeInterval;
    public Animator Anim;
    public Sprite[] Sprites;
    public SpriteAtlas Atlas;

    private float angleTimer;
    private float changeTimer;
    private float currentInterval;
    private Vector3 oldAngle;
    private int spriteIndex;
    private SpriteRenderer[] sprRenderers;

    public void Start()
    {
        sprRenderers = GetComponents<SpriteRenderer>();
        oldAngle = transform.localEulerAngles;
        spriteIndex = RandomSpriteIndex();
        SetNewSprite();
    }

    public void Update()
    {
        // Rotation.
        angleTimer += Time.unscaledDeltaTime;
        if(angleTimer > currentInterval)
        {
            oldAngle = RandomRot;
            NewRandomRotation();
            angleTimer -= currentInterval;
            currentInterval = Random.Range(Interval.x, Interval.y);
        }

        LerpToTarget();

        // Transition.
        changeTimer += Time.unscaledDeltaTime;
        if(changeTimer > ChangeInterval)
        {
            changeTimer -= ChangeInterval;

            int newIndex;
            do
            {
                newIndex = RandomSpriteIndex();

            } while (newIndex == spriteIndex);

            spriteIndex = newIndex;

            // Trigger animation...
            Anim.SetTrigger("Transition");

            // The sprite from the newly selected index is applied during the animation using a callback.
        }
    }

    public void SetNewSprite()
    {
        Sprite s = Sprites[spriteIndex];
        foreach (var spr in sprRenderers)
        {
            Sprite final = Atlas.GetSprite(s.name);
            spr.sprite = final;
        }
    }

    public int RandomSpriteIndex()
    {
        return Random.Range(0, Sprites.Length);
    }

    public void NewRandomRotation()
    {
        float x = Random.Range(-Magnitude, Magnitude) * 0.4f;
        float y = Random.Range(-Magnitude, Magnitude);
        float z = Random.Range(-Magnitude, Magnitude) * 0.4f;
        RandomRot = new Vector3(x, y, z);
    }

    private void LerpToTarget()
    {
        Vector3 newRot = LerpAngle(oldAngle, RandomRot, angleTimer / currentInterval);

        transform.localEulerAngles = newRot;
    }

    private Vector3 LerpAngle(Vector3 a, Vector3 b, float t)
    {
        float x = Mathf.LerpAngle(a.x, b.x, t);
        float y = Mathf.LerpAngle(a.y, b.y, t);
        float z = Mathf.LerpAngle(a.z, b.z, t);

        return new Vector3(x, y, z);
    }
}