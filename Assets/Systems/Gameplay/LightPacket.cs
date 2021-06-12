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

    GameObject visual;
    Vector3 position;
    Vector3 next;
    int nextIndex;
    Vector3 nextPartial;

    public int NextIndex => nextIndex;
    public Vector3 Next => next;
    public Vector3 Position => position;
    public GameObject Visual => visual;

    public LightPacket Setup(GameObject visual, Vector3 previous, Vector3 next, int nextIndex)
    {
        this.visual = visual;
        this.next = next;
        this.position = previous;
        this.nextIndex = nextIndex;

        visual.transform.position = position;

        return this;
    }

    public void Clear()
    {
        this.visual = null;
        this.next = Vector3.zero;
        this.position = Vector3.zero;
        this.nextIndex = 0;
    }

    public void SetNext(Vector3 next, int nextIndex)
    {
        this.next = next;
        this.nextIndex = nextIndex;
    }

    public void SetPosition(Vector3 pos)
    {
        this.position = pos;
        visual.transform.position = this.position;
    }

    public State Tick()
    {
        float distanceLeft = Vector3.Distance(position, next);
        float distanceToMove = Mathf.Min(GameConstants.LightPacketSpeed, distanceLeft);
        position = Vector3.MoveTowards(position, next, distanceToMove);

        visual.transform.position = position;

        nextPartial = Vector3.MoveTowards(position, next, distanceToMove);

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

    public void Update()
    {
        visual.transform.position = Vector3.Lerp(
            visual.transform.position,
            nextPartial,
            Mathf.SmoothStep(0f, 1f, Mathf.Pow(Time.deltaTime, GameConstants.LightPacketSpeed))
        );
    }
}