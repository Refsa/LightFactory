using System.Collections;
using System.Collections.Generic;
using Refsa.EventBus;
using UnityEngine;

public class LightUpgrader : MonoBehaviour, ITicker, ILaserCollector
{
    [SerializeField] Color upgraderFor = Color.white;
    [SerializeField] LightLevel upgradeFrom = LightLevel.One;
    [SerializeField] LightLevel upgradeTo = LightLevel.Two;
    [SerializeField] int combineRate = 2;

    [AutoBind] LaserSource laserSource;

    int received;
    int connections;
    int lastCombine;

    public int TickerPriority => TickerPriorities.LASER_COLLECTOR;

    void Start()
    {
        OnEnable();
        laserSource.SetColor(upgraderFor);
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
        if (color != upgraderFor)
        {
            return;
        }

        if (lightPacket.LightLevel != upgradeFrom)
        {
            return;
        }

        received++;
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

    public void Tick(int tick)
    {
        if (connections != 2) return;

        if (tick - lastCombine > combineRate)
        {
            lastCombine = tick;

            if (received < 2)
            {
                return;
            }

            received -= 2;

            laserSource.NewPacket(upgradeTo);
            laserSource.Tick(tick);
        }
    }
}
