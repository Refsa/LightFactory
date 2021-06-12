using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Refsa.EventBus;
using UnityEngine;

public class LightCombiner : MonoBehaviour, ITicker, ILaserCollector
{
    [SerializeField] int collectorCount;
    [SerializeField] int combineRate = 2;

    [AutoBind] LaserSource laserSource;

    int[] colorStorage;
    HashSet<Color> activeColors;
    int lastCombine;

    public int TickerPriority => TickerPriorities.LIGHT_COMBINER;

    void Start()
    {
        colorStorage = new int[7];
        activeColors = new HashSet<Color>();
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

    void ClearStorage()
    {
        for (int i = 0; i < 6; i++)
        {
            colorStorage[i] = 0;
        }
    }

    public void Notify(Color color)
    {
        activeColors.Add(color);
        colorStorage[GameConstants.ColorToID(color)]++;
    }

    public void NotifyConnected(GameObject source)
    {
        
    }

    public void Tick(int tick)
    {
        if (activeColors.Count < collectorCount)
        {
            laserSource.Disable();
        }
        else if (activeColors.Count > collectorCount)
        {
            activeColors.Clear();
            ClearStorage();
        }
        else
        {
            laserSource.Enable();
            Color combinedColor = activeColors.Aggregate(Color.black, (acc, val) => acc += val);
            combinedColor.a = 1f;
            laserSource.SetColor(combinedColor);

            if (tick - lastCombine >= combineRate)
            {
                lastCombine = tick;

                bool hasCapacity = true;
                foreach (var color in activeColors)
                {
                    hasCapacity = hasCapacity && colorStorage[GameConstants.ColorToID(color)] > 0;
                }

                if (!hasCapacity) return;

                foreach (var color in activeColors)
                {
                    int index = GameConstants.ColorToID(color);
                    colorStorage[index]--;
                }

                laserSource.NewPacket();
                laserSource.Tick(tick);
            }
        }
    }

    public void NotifyConnected(Connection connection)
    {
        
    }

    public void NotifyDisconnected(Connection connection)
    {
        
    }
}
