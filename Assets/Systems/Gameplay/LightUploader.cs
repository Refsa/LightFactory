using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Refsa.EventBus;
using UnityEngine;

public class LightUploader : MonoBehaviour, ITicker, ILaserCollector
{
    [SerializeField] int uploadRate = 10;
    [SerializeField] LightLevel forLevel;

    Dictionary<LightMeta, int> storage;
    int lastUpload;

    public int TickerPriority => TickerPriorities.LASER_COLLECTOR;

    void Start()
    {
        storage = new Dictionary<LightMeta, int>()
        {
            {new LightMeta(forLevel, LightColor.Red), 0},
            {new LightMeta(forLevel, LightColor.Blue), 0},
            {new LightMeta(forLevel, LightColor.Green), 0},
            {new LightMeta(forLevel, LightColor.Cyan), 0},
            {new LightMeta(forLevel, LightColor.Magenta), 0},
            {new LightMeta(forLevel, LightColor.Yellow), 0},
        };
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

            for (int i = 0; i < storage.Count; i++)
            {
                var kvp = storage.ElementAt(i);
                GlobalEventBus.Bus.Pub(new LightInventoryChange(kvp.Key.LightLevel, kvp.Key.LightColor, kvp.Value));

                storage[kvp.Key] = 0;
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

        var key = storage.Keys.Where(e => e.LightColor == color.ToColor()).FirstOrDefault();
        if (key == null)
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
