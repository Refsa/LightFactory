using System.Collections;
using System.Collections.Generic;
using Refsa.EventBus;
using UnityEngine;

public class StorageExtractor : MonoBehaviour, ITicker
{
    [SerializeField] LaserSource laserSource;
    [SerializeField] LineRenderer storageConnection;
    [SerializeField] int extractionRate = 2;
    [SerializeField] float range = 2f;

    LightStorage lightStorage;
    int lastExtraction;

    int solidLayerMask;

    public int TickerPriority => TickerPriorities.LASER_COLLECTOR;

    void Start()
    {
        storageConnection.enabled = false;
        solidLayerMask = 1 << LayerMask.NameToLayer("Solid");
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
        if (lightStorage == null || (Vector3.Distance(transform.position, lightStorage.transform.position) > range))
        {
            var checkHit = Physics2D.OverlapCircle(transform.position, range, solidLayerMask);
            if (checkHit != null)
            {
                if (checkHit.gameObject.HasTag("LightStorage"))
                {
                    lightStorage = checkHit.gameObject.GetComponent<LightStorage>();
                }
                else if (checkHit.gameObject.HasTagInParent("LightStorage"))
                {
                    lightStorage = checkHit.gameObject.GetComponentInParent<LightStorage>();
                }
                else
                {
                    storageConnection.enabled = false;
                    return;
                }

            }
            else
            {
                storageConnection.enabled = false;
                return;
            }
        }

        storageConnection.enabled = true;
        storageConnection.positionCount = 2;
        storageConnection.SetPosition(0, transform.position);
        storageConnection.SetPosition(1, lightStorage.transform.position);

        if (tick - lastExtraction > extractionRate)
        {
            lastExtraction = tick;

            var packetAvailable = lightStorage.Drain();
            if (packetAvailable.HasValue)
            {
                laserSource.SetColor(packetAvailable.Value.Item2);
                laserSource.NewPacket(packetAvailable.Value.Item1);

                storageConnection.startColor = packetAvailable.Value.Item2;
                storageConnection.endColor = packetAvailable.Value.Item2;
            }
            else
            {
                storageConnection.startColor = Color.gray;
                storageConnection.endColor = Color.gray;
            }
        }
    }
}
