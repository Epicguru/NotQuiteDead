using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReverbArea : MonoBehaviour {

    public AudioReverbPreset Reverb;
    public Vector2 Size = new Vector2(1, 1);

    public void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position - (Vector3)Size / 2f, new Vector3(transform.position.x + Size.x / 2f, transform.position.y - Size.y / 2f, 0f));
        Gizmos.DrawLine(transform.position - (Vector3)Size / 2f, new Vector3(transform.position.x - Size.x / 2f, transform.position.y + Size.y / 2f, 0f));
        Gizmos.DrawLine(transform.position + (Vector3)Size / 2f, new Vector3(transform.position.x - Size.x / 2f, transform.position.y + Size.y / 2f, 0f));
        Gizmos.DrawLine(transform.position + (Vector3)Size / 2f, new Vector3(transform.position.x + Size.x / 2f, transform.position.y - Size.y / 2f, 0f));
    }

    public Rect GetRekt()
    {
        // OOHOHOHOOOHOHOH MUM GET THE CAMERA!
        // Very mature of me, I know. I even stay up when my mummy tells me to go to bed.

        return new Rect(transform.position - (Vector3)Size / 2f, Size);
    }

    public void LateUpdate()
    {
        if (GetRekt().Contains(Camera.main.transform.position, true))
        {
            // Apply effect
            Camera.main.GetComponent<CameraReverbControl>().SetReverb(Reverb);
        }
    }
}
