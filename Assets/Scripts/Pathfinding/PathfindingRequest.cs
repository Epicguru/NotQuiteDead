
using System.Collections.Generic;
using UnityEngine.Events;

public struct PathfindingRequest
{
    public int StartX;
    public int StartY;
    public int EndX;
    public int EndY;
    public TileLayer Layer;
    public UnityAction<List<Node>> Done;

    public bool IsValid()
    {
        if (Layer == null)
            return false;
        if (Done == null)
            return false;

        if(!Layer.InLayerBounds(EndX, EndY))
        {
            return false;
        }

        return true;
    }
}