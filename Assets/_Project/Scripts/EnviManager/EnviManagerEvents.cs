public class OnBarrelPouring : GameEvent
{
    public int laneIndex;
    public bool isPouring;

    public OnBarrelPouring(int laneIndex, bool isPouring)
    {
        this.laneIndex = laneIndex;
        this.isPouring = isPouring;
    }


}

public class OnKeranPouring : GameEvent
{
    public bool isPouring;

    public OnKeranPouring(bool isPouring)
    {
        this.isPouring = isPouring;
    }
}
