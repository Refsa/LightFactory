using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightStorage : MonoBehaviour, ITicker, ILaserCollector
{
    public int TickerPriority => TickerPriorities.LASER_COLLECTOR;

    [SerializeField] int _storage;
    [SerializeField] Color _color;
    [SerializeField] SpriteRenderer collectorPoint;

    Color activeColor;
    int storage;

    public void Tick(int tick)
    {

    }

    public void Notify(Color color)
    {
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
