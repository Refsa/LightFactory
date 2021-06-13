using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightStorage : MonoBehaviour, ITicker, ILaserCollector
{
    public int TickerPriority => TickerPriorities.LASER_COLLECTOR;

    [SerializeField] int _storage;
    [SerializeField] Color _color;
    [SerializeField] SpriteRenderer collectorPoint;


    LightLevel activeLevel;
    Color activeColor;
    int storage;

    public void Tick(int tick)
    {

    }

    public (LightLevel, Color)? Drain(int count = 1)
    {
        if (storage == 0)
        {
            return null;
        }

        storage--;
        return (activeLevel, activeColor);
    }

    public void Notify(Color color, LightPacket lightPacket)
    {
        if (activeColor.a == 0f)
        {
            activeColor = color;
            collectorPoint.color = activeColor;
        }

        if (activeColor != color || lightPacket.LightLevel != activeLevel)
        {
            storage = 0;
            activeColor = color;
            collectorPoint.color = activeColor;
            activeLevel = lightPacket.LightLevel;
        }

        storage++;
        _storage = storage;
        _color = color;
    }

    public void NotifyConnected(Connection connection)
    {

    }

    public void NotifyDisconnected(Connection connection)
    {

    }
}
