
using System.Collections.Generic;
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
    public float ShotSpeedMultiplier = 1f;

    [Header("Reloading")]
    public float ReloadSpeedMultiplier = 1f;

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

        // Animation
        gun.Shooting.ReloadSpeedMultiplier *= ReloadSpeedMultiplier;
        gun.Shooting.ShootSpeedMultiplier *= ShotSpeedMultiplier;
    }

    public string[] GetEffects()
    {
        List<string> eff = new List<string>();
        bool pos;

        // Damage
        if(DamageMultiplier != 1f)
        {
            pos = DamageMultiplier > 1f;
            eff.Add("Base Damage " + RichText.InColour((pos ? "+" : "-") + Mathf.RoundToInt(Mathf.Abs(DamageMultiplier - 1f) * 100f) + "%", pos ? Color.green : Color.red));
        }

        // Damage Falloff
        if (DamageFalloffMultiplier != 1f)
        {
            pos = DamageFalloffMultiplier > 1f;
            eff.Add("Damage Falloff " + RichText.InColour((pos ? "-" : "+") + Mathf.RoundToInt(Mathf.Abs(DamageFalloffMultiplier - 1f) * 100f) + "%", pos ? Color.green : Color.red));
        }

        // Penetration
        if (PenetrationChange != 0)
        {
            pos = PenetrationChange >= 1;
            eff.Add("Penetration " + RichText.InColour((pos ? "+" : "-") + Mathf.Abs(PenetrationChange), pos ? Color.green : Color.red));
        }

        // Penetration Falloff
        if (PenetrationFalloffMultiplier != 1f)
        {
            pos = PenetrationFalloffMultiplier > 1f;
            eff.Add("Penetration Falloff " + RichText.InColour((pos ? "-" : "+") + Mathf.RoundToInt(Mathf.Abs(PenetrationFalloffMultiplier - 1f) * 100f) + "%", pos ? Color.green : Color.red));
        }

        // Magazine Capacity
        if (MagazineCapacityMultiplier != 1f)
        {
            pos = MagazineCapacityMultiplier > 1f;
            eff.Add("Magazine Capacity " + RichText.InColour((pos ? "+" : "-") + Mathf.RoundToInt(Mathf.Abs(MagazineCapacityMultiplier - 1f) * 100f) + "%", pos ? Color.green : Color.red));
        }

        // Inaccuracy
        if (InaccuracyMultiplier.x == InaccuracyMultiplier.y)
        {
            if(InaccuracyMultiplier.x != 1f)
            {
                pos = InaccuracyMultiplier.x < 1f;
                eff.Add("Accuracy " + RichText.InColour((pos ? "+" : "-") + Mathf.RoundToInt(Mathf.Abs(InaccuracyMultiplier.x - 1f) * 100f) + "%", pos ? Color.green : Color.red));
            }
        }
        else
        {
            if (InaccuracyMultiplier.x != 1f)
            {
                pos = InaccuracyMultiplier.x < 1f;
                eff.Add("Initial Accuracy " + RichText.InColour((pos ? "+" : "-") + Mathf.RoundToInt(Mathf.Abs(InaccuracyMultiplier.x - 1f) * 100f) + "%", pos ? Color.green : Color.red));
            }
            if (InaccuracyMultiplier.y != 1f)
            {
                pos = InaccuracyMultiplier.y < 1f;
                eff.Add("Final Accuracy " + RichText.InColour((pos ? "+" : "-") + Mathf.RoundToInt(Mathf.Abs(InaccuracyMultiplier.y - 1f) * 100f) + "%", pos ? Color.green : Color.red));
            }
        }

        // Shot Speed
        if (ShotSpeedMultiplier != 1f)
        {
            pos = ShotSpeedMultiplier > 1f;
            eff.Add("Fire Rate " + RichText.InColour((pos ? "+" : "-") + Mathf.RoundToInt(Mathf.Abs(ShotSpeedMultiplier - 1f) * 100f) + "%", pos ? Color.green : Color.red));
        }

        // Reload Speed
        if (ReloadSpeedMultiplier != 1f)
        {
            pos = ReloadSpeedMultiplier > 1f;
            eff.Add("Reload Speed " + RichText.InColour((pos ? "+" : "-") + Mathf.RoundToInt(Mathf.Abs(ReloadSpeedMultiplier - 1f) * 100f) + "%", pos ? Color.green : Color.red));
        }

        // Range
        if (RangeMultiplier != 1f)
        {
            pos = RangeMultiplier > 1f;
            eff.Add("Range " + RichText.InColour((pos ? "+" : "-") + Mathf.RoundToInt(Mathf.Abs(RangeMultiplier - 1f) * 100f) + "%", pos ? Color.green : Color.red));
        }

        return eff.ToArray();
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