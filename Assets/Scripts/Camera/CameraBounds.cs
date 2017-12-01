
using UnityEngine;

public class CameraBounds : MonoBehaviour
{
    public static CameraBounds Instance;

    public Camera Cam;
    public Rect Bounds;

    public void Awake()
    {
        Instance = this;
    }

    public void OnDestroy()
    {
        Instance = null;
    }

    public void LateUpdate()
    {
        if(Cam == null)
        {
            return;
        }

        // Get position.
        Vector2 position = Cam.transform.position;

        // X and Y of that position.
        float x = position.x;
        float y = position.y;

        // Orthographic size, height.
        float size = Cam.orthographicSize;

        // Ratio of width to height.
        float ratio = (float)Screen.width / Screen.height;

        // Calculate final width.
        float width = size * ratio;

        // Height is equal to the orthographic size.
        float height = size;

        // Get the bottom left corner of the bounds.
        float bottomX = x - width;
        float bottomY = y - height;

        // Apply to bounds.
        Bounds.x = bottomX;
        Bounds.y = bottomY;
        Bounds.width = width * 2f;
        Bounds.height = height * 2f;
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(new Vector3(Bounds.xMin, Bounds.yMin, 0), 0.1f);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(new Vector3(Bounds.xMax, Bounds.yMax, 0), 0.1f);
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(new Vector3(Bounds.center.x, Bounds.center.y, 0), 0.1f);
    }
}
