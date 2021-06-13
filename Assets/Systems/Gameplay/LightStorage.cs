using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightStorage : MonoBehaviour, ITicker, ILaserCollector
{
    public int TickerPriority => TickerPriorities.LASER_COLLECTOR;

    [SerializeField] int _storage;
    [SerializeField] Color _color;
    [SerializeField] SpriteRenderer collectorPoint;
    [SerializeField] LightLevel forLevel;


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
        return (forLevel, activeColor);
    }

    public void Notify(Color color, LightPacket lightPacket)
    {
        if (lightPacket.LightLevel != forLevel)
        {
            // TODO: feedback on wrong light level
            return;
        }

        if (activeColor.a == 0f)
        {
            activeColor = color;
            collectorPoint.color = activeColor;
        }

        if (activeColor != color)
        {
            storage = 0;
            activeColor = color;
            collectorPoint.color = activeColor;
            // TODO: Feedback on color change
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
