
using System.Collections.Generic;
using UnityEngine.Events;

public struct PathfindingRequest
{
    public string ID;
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
        if (ID == null)
            return false;

        if(!Layer.InLayerBounds(EndX, EndY))
        {
            return false;
        }

        if (!Layer.IsSpotWalkable(StartX, StartY))
        {
            // Within a solid tile.
            return false;
        }

        if (!Layer.IsSpotWalkable(EndX, EndY))
        {
            // Within a solid tile.
            return false;
        }

        return true;
    }
}