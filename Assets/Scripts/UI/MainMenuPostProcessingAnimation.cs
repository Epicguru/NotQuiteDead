using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[RequireComponent(typeof(PostProcessVolume))]
public class MainMenuPostProcessingAnimation : MonoBehaviour
{
    [Header("Chromatic Aberration")]
    public bool ChromaticEffectActive;
    public float NormalChromaticIntensity;
    public Vector2 EffectChromaticIntensity;

    PostProcessProfile v;

    void OnEnable()
    {
        var behaviour = GetComponent<PostProcessVolume>();

        if (behaviour.profile == null)
        {
            enabled = false;
            return;
        }

        v = Instantiate(behaviour.profile);
        behaviour.profile = v;
    }

    void Update()
    {
        ChromaticAberration chromatic;
        bool worked = v.TryGetSettings(out chromatic);

        if (worked)
        {
            if (ChromaticEffectActive)
            {
                chromatic.intensity.value = Random.Range(EffectChromaticIntensity.x, EffectChromaticIntensity.y);
            }
            else
            {
                chromatic.intensity.value = NormalChromaticIntensity;
            }
        }
    }
}