using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Refsa.EventBus;
using UnityEngine;

public class LightUploader : MonoBehaviour, ITicker, ILaserCollector
{
    [SerializeField] int uploadRate = 10;
    [SerializeField] LightLevel forLevel;

    int[] storage;
    int lastUpload;

    public int TickerPriority => TickerPriorities.LASER_COLLECTOR;

    void Start()
    {
        storage = new int[7];
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
        if (tick - lastUpload > uploadRate)
        {
            lastUpload = tick;

            for (int i = 0; i < storage.Length; i++)
            {
                GlobalEventBus.Bus.Pub(new LightInventoryChange(forLevel, GameConstants.IDToLightColor(i), storage[i]));
                storage[i] = 0;
            }
        }
    }

    public void Notify(Color color, LightPacket lightPacket)
    {
        if (lightPacket.LightLevel != forLevel)
        {
            // TODO: Feedback on wrong packet level
            return;
        }

        var key = GameConstants.ColorToID(color);
        if (key == -1)
        {
            throw new System.ArgumentOutOfRangeException();
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
