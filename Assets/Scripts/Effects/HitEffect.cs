using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitEffect : MonoBehaviour
{
    public float FPS = 10f;
    public Color Colour = Color.white;
    public Sprite[] Sprites;
    public SpriteRenderer Renderer;

    private float timer;
    private int index;

    public void Init(Vector2 position, float rotation, float FPS, Color colour, Sprite[] sprites)
    {
        index = 0;
        timer = 0;
        Colour = colour;
        this.FPS = FPS;
        Sprites = sprites;

        transform.position = position;
        transform.rotation = Quaternion.identity;
        transform.Rotate(0, 0, rotation);
    }

    public void Update()
    {
        timer += Time.deltaTime;

        float interval = GetInterval();
        bool done = false;

        while(timer >= interval)
        {
            timer -= interval;
            index++;

            if(index >= Sprites.Length)
            {
                done = true;
                break;
            }

            Renderer.sprite = Sprites[index];
        }

        Renderer.color = GetColour();

        if (done)
        {
            ObjectPool.Destroy(gameObject, PoolType.HIT_EFFECT);
            return;
        }
    }

    public virtual float GetInterval()
    {
        return 1f / FPS;
    }

    public virtual Color GetColour()
    {
        return this.Colour;
    }

    public static void Spawn(Vector2 position, float rotation, float FPS, Color colour, Sprite[] sprites)
    {
        GameObject prefab = Spawnables.I.HitEffect;
        GameObject go = ObjectPool.Instantiate(prefab, PoolType.HIT_EFFECT);

        HitEffect effect = go.GetComponent<HitEffect>();
        effect.Init(position, rotation, FPS, colour, sprites);
    }

    public static void Spawn(Vector2 position, float rotation, HitEffectPreset preset)
    {
        Spawn(position, rotation, preset.FPS, preset.Colour, preset.Sprites);
    }

    public static void Spawn(Vector2 position, float rotation, Collider2D collisionSurface, bool playAudio)
    {
        // Using a collision surface, look for an appropriate CollisionSurface component in the parent.
        CollisionSurface surface = collisionSurface.GetComponentInParent<CollisionSurface>();

        if(surface != null)
        {
            // Spawn effect, if present.
            HitEffectPreset effect = surface.GetHitEffect();
            if(effect != null)
            {
                Spawn(position, rotation, effect);
            }

            // Now play audio using the settings on the CollisionSurface
            AudioClip c = surface.GetAudioClip();
            if(c != null)
            {
                float volume = surface.GetVolume();
                float pitch = surface.GetPitch();
                float range = surface.GetRange();
                float maxPanRange = surface.GetMaxPanRange();
                float minPanRange = surface.GetMinPanRange();
                float lowPassStart = surface.GetLowPassDistance();

                AudioManager.Instance.PlayOneShot(position, c, volume, pitch, range, minPanRange, maxPanRange, lowPassStart);
            }
        }
    }
}