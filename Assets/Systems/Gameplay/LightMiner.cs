using System.Collections;
using System.Collections.Generic;
using Refsa.EventBus;
using UnityEngine;

public class LightMiner : MonoBehaviour, ILaserCollector, ITicker
{
    [SerializeField] int miningRate;

    [AutoBind] LaserSource laserSource;

    bool gotPacket;
    Connection connection;

    public int TickerPriority => TickerPriorities.LASER_COLLECTOR;

    void Start()
    {
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

    public void Notify(Color color, LightPacket lightPacket)
    {
        laserSource.SetColor(color);
        gotPacket = true;
    }

    public void Tick(int tick)
    {
        if (gotPacket)
        {
            laserSource.NewPacket();
            gotPacket = false;
        }
    }

    public void NotifyConnected(Connection connection)
    {
        this.connection = connection;
        connection.Src.SetRate(miningRate);
    }

    public void NotifyDisconnected(Connection connection)
    {
        this.connection = null;
        connection.Src.ResetRate();
    }
}
