using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[System.Serializable]
public class GunAudio
{
    public AudioClip[] Clips;
    public Vector2 Volume = new Vector2(1, 1);
    public Vector2 Pitch = new Vector2(1, 1);

    public bool RequireNewRandom = true;

    private int oldClip;

    public AudioClip GetRandomClip()
    {
        if(Clips.Length == 0)
        {
            return null;
        }
        if (!RequireNewRandom)
        {
            int random = Random.Range(0, Clips.Length);
            oldClip = random; // Just in case we swap requirements at runtime.
            return Clips[random];
        }
        else
        {
            // Stop if there is only one clip.
            if (Clips.Length == 1)
                return Clips[0];

            // Requires new random...
            int random = Random.Range(0, Clips.Length);
            while(random == oldClip)
            {
                random = Random.Range(0, Clips.Length);
            }

            // We have new random
            oldClip = random;
            return Clips[random];
        }
    }

    public float GetRandomVolume()
    {
        return Random.Range(Volume.x, Volume.y);
    }

    public float GetRandomPitch()
    {
        return Random.Range(Pitch.x, Pitch.y);
    }
}
