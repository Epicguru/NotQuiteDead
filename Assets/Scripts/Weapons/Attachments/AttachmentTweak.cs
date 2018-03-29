
using UnityEngine;

[RequireComponent(typeof(Attachment))]
public class AttachmentTweak : MonoBehaviour
{
    [Header("Damage")]
    public float DamageMultiplier = 1f;
    public float DamageFalloffMultiplier = 1f;
    public int PenetrationChange = 0;
    public float PenetrationFalloffMultiplier = 1f;

    [Header("Capacity")]
    public float MagazineCapacityMultiplier = 1f;

    [Header("Shooting")]
    [Tooltip("X value is the multiplier for the initial (best) inaccuracy. Y value is the multiplier for the final (worst) inaccuracy.")]
    public Vector2 InaccuracyMultiplier = new Vector2(1f, 1f);

    [Header("Range")]
    public float RangeMultiplier = 1f;

    [Header("Audio")]
    public float AudioRangeMultiplier = 1f;
    public float AudioVolumeMultiplier = 1f;
    public float AudioPitchMultiplier = 1f;

    [Header("Custom Shot")]
    public AudioClip[] CustomShotSounds;
    public float CustomShotRange = 40f;
    public Vector2 CustomShotVolume = new Vector2(1f, 1f);
    public Vector2 CustomShotPitch = new Vector2(1f, 1f);

    public Attachment Attachment
    {
        get
        {
            if(_A == null)
            {
                _A = GetComponent<Attachment>();
            }
            return _A;
        }
    }
    private Attachment _A;

    public void Awake()
    {
        Attachment.UponShoot.AddListener(OnShoot);
    }

    public void Apply(Gun gun)
    {
        float x;

        // Damage
        x = gun.Shooting.Damage.Damage;
        x = Mathf.Max(x * DamageMultiplier, 1f);
        gun.Shooting.Damage.Damage = x;

        // Damage falloff
        x = gun.Shooting.Damage.DamageFalloff;
        x = Mathf.Clamp(x * DamageFalloffMultiplier, 0f, 1f);
        gun.Shooting.Damage.DamageFalloff = x;

        // Penetration change
        x = gun.Shooting.Damage.Penetration;
        x += PenetrationChange;
        x = Mathf.Max(x, 1);
        gun.Shooting.Damage.Penetration = (int)x;

        // Penetration falloff
        x = gun.Shooting.Damage.PenetrationFalloff;
        x = Mathf.Clamp(x * PenetrationFalloffMultiplier, 0f, 1f);
        gun.Shooting.Damage.PenetrationFalloff = x;

        // Magazine capacity
        x = gun.Shooting.Capacity.MagazineCapacity;
        x *= MagazineCapacityMultiplier;
        gun.Shooting.Capacity.MagazineCapacity = Mathf.Max(Mathf.RoundToInt(x), 1);

        // Inaccuracy
        gun.Shooting.Damage.Inaccuracy.x *= InaccuracyMultiplier.x;
        gun.Shooting.Damage.Inaccuracy.y *= InaccuracyMultiplier.y;

        // Range
        gun.Shooting.Damage.Range *= RangeMultiplier;

        // Audio
        gun.Shooting.AudioSauce.RangeMultiplier *= AudioRangeMultiplier;
        gun.Shooting.AudioSauce.VolumeMultiplier *= AudioVolumeMultiplier;
        gun.Shooting.AudioSauce.PitchMultiplier *= AudioPitchMultiplier;
    }

    public void OnShoot()
    {
        if(CustomShotSounds != null && CustomShotSounds.Length > 0)
        {
            AudioClip sound = CustomShotSounds[Random.Range(0, CustomShotSounds.Length)];
            if(sound != null)
            {
                float range = CustomShotRange;
                float volume = Random.Range(CustomShotVolume.x, CustomShotVolume.y);
                float pitch = Random.Range(CustomShotPitch.x, CustomShotPitch.y);

                // Play custom sound!
                AudioManager.Instance.PlayOneShot(Attachment.Gun.transform.position, sound, volume, pitch, range);
            }
        }
    }
}