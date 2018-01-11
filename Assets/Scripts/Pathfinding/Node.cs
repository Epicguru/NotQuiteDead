using Priority_Queue;

public class Node : FastPriorityQueueNode
{
    public int X;
    public int Y;

    public override bool Equals(object obj)
    {
        Node other = (Node)obj;
        return other.X == X && other.Y == Y;
    }

    public override int GetHashCode()
    {
        return X + Y * 7;
    }
}