using UnityEngine;

public class LightPacketPooler : Pooler<LightPacket>
{
    protected override LightPacket Producer()
    {
        return new LightPacket();
    }
}

public class LightPacket
{
    public enum State
    {
        None = 0,
        More,
        Done,
    }

    const float distancePerTick = 1f;

    GameObject visual;
    Vector3 position;
    Vector3 next;
    int nextIndex;

    public int NextIndex => nextIndex;
    public Vector3 Next => next;
    public GameObject Visual => visual;

    public LightPacket Setup(GameObject visual, Vector3 previous, Vector3 next, int nextIndex)
    {
        this.visual = visual;
        this.next = next;
        this.position = previous;
        this.nextIndex = nextIndex;

        return this;
    }

    public void SetNext(Vector3 next, int nextIndex)
    {
        this.next = next;
        this.nextIndex = nextIndex;
    }

    public State Tick()
    {
        float distanceLeft = Vector3.Distance(position, next);
        float distanceToMove = Mathf.Min(distancePerTick, distanceLeft);
        position = Vector3.MoveTowards(position, next, distanceToMove);

        visual.transform.position = position;

        distanceLeft -= distanceToMove;
        if (distanceLeft < 0f)
        {
            return State.More;
        }
        else if (Mathf.Approximately(0f, distanceLeft))
        {
            return State.Done;
        }

        return State.None;
    }
}