using UnityEngine;

public class LightMesh : MonoBehaviour
{
    public static int UpdateCount;

    public LightMeshGen Gen;
    public LightMeshInteraction Interaction;
    public LightMeshChunk Chunk;

    [ReadOnly]
    public bool Dirty;

    public void Start()
    {
        // No need to generate mesh every time, it is saved as a prefab.
        // Will not generate normally, see inside.
        Gen.GenMesh();
    }

    public void UpdateLighting()
    {
        Dirty = true;
    }

    public void LateUpdate()
    {
        // Update lighting if dirty...
        if (Dirty)
        {
            Dirty = false;
            UpdateAndApplyLighting();
        }
    }

    private void UpdateAndApplyLighting()
    {
        Chunk.ApplyAmbientLight();
        UpdateCount++;
    }
}