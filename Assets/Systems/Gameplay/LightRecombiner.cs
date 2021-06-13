using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Refsa.EventBus;
using UnityEngine;

public class LightRecombiner : MonoBehaviour, ITicker, ILaserCollector
{
    [SerializeField] int combineRate = 2;
    [SerializeField] LightLevel forLevel;
    [SerializeField] LaserSource laserSource;

    int lastRecombine;
    int[] storage;

    public int TickerPriority => TickerPriorities.LASER_COLLECTOR;

    void Start()
    {
        laserSource.SetColor(Color.white);

        storage = new int[6];
        OnEnable();
    }

    void OnEnable()
    {
        GlobalEventBus.Bus.Pub(new RegisterTicker(this));
    }

    void OnDisable()
    {
        GlobalEventBus.Bus.Pub(new UnregisterTicker(this));
    }

    public void Tick(int tick)
    {
        bool hasStorage = storage.All(e => e > 0);

        if (!hasStorage)
        {
            // TODO: Warn of missing storage
            return;
        }

        if (tick - lastRecombine < combineRate) return;
        lastRecombine = tick;

        for (int i = 0; i < storage.Length; i++)
        {
            storage[i]--;
        }

        laserSource.NewPacket(forLevel);
    }

    public void Notify(Color color, LightPacket lightPacket)
    {
        if (lightPacket.LightLevel != forLevel)
        {
            // TODO: warn of wrong light level
            return;
        }

        var key = GameConstants.ColorToID(color);
        if (key == -1)
        {
            throw new System.ArgumentOutOfRangeException(color.ToString());
        }

        storage[key]++;
    }

    public void NotifyConnected(Connection connection)
    {

    }

    public void NotifyDisconnected(Connection connection)
    {

    }
}
