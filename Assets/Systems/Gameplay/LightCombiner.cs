using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Refsa.EventBus;
using UnityEngine;

public class LightCombiner : MonoBehaviour, ITicker, ILaserCollector
{
    [SerializeField] int combineRate = 2;
    [SerializeField] Color combineA = Color.white;
    [SerializeField] Color combineB = Color.white;
    [SerializeField] LightLevel forLevel;

    [AutoBind] LaserSource laserSource;

    int colorStorageA = 0;
    int colorStorageB = 0;

    int lastCombine;
    int connections = 0;

    public int TickerPriority => TickerPriorities.LIGHT_COMBINER;

    void Start()
    {
        OnEnable();

        Color combinedColor = GameConstants.CombineColor(combineA, combineB);
        combinedColor.a = 1f;
        laserSource.SetColor(combinedColor);
    }

    void OnEnable()
    {
        GlobalEventBus.Bus.Pub(new RegisterTicker(this));
    }

    void OnDisable()
    {
        GlobalEventBus.Bus.Pub(new UnregisterTicker(this));
    }

    void ClearStorage()
    {
        colorStorageA = 0;
        colorStorageB = 0;
    }

    public void Notify(Color color, LightPacket lightPacket)
    {
        if (lightPacket.LightLevel != forLevel)
        {
            // TODO: feedback on wrong light level
            return;
        }

        if (color == combineA)
        {
            colorStorageA++;
        }
        else if (color == combineB)
        {
            colorStorageB++;
        }
    }

    public void NotifyConnected(GameObject source)
    {

    }

    public void Tick(int tick)
    {
        if (tick - lastCombine >= combineRate)
        {
            lastCombine = tick;

            bool hasCapacity = colorStorageA > 0 && colorStorageB > 0;

            if (!hasCapacity) return;

            colorStorageA--;
            colorStorageB--;

            laserSource.NewPacket();
            laserSource.Tick(tick);
        }
    }

    public void NotifyConnected(Connection connection)
    {
        connections++;

        if (connections == 2)
        {
            laserSource.Enable();
        }
    }

    public void NotifyDisconnected(Connection connection)
    {
        connections--;

        if (connections != 2)
        {
            laserSource.Disable();
        }
    }
}
