using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserCollector : MonoBehaviour, ITicker
{
    public int TickerPriority => TickerPriorities.LASER_COLLECTOR;

    [SerializeField] int _storage;
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
    }
}
