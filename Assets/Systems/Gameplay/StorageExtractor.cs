using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Refsa.EventBus;
using UnityEngine;

public class StorageExtractor : MonoBehaviour, ITicker
{
    [SerializeField] LaserSource laserSource;
    [SerializeField] LineRenderer storageConnection;
    [SerializeField] Transform storageConnectorPoint;
    [SerializeField] int extractionRate = 2;
    [SerializeField] float range = 2f;

    LightStorage lightStorage = null;
    int lastExtraction;
    bool isConnected;

    public int TickerPriority => TickerPriorities.LASER_COLLECTOR;

    void Start()
    {
        OnEnable();
        storageConnection.gameObject.SetActive(false);
    }

    void OnEnable()
    {
        lightStorage = null;
        GlobalEventBus.Bus.Pub(new RegisterTicker(this));
    }

    void OnDisable()
    {
        GlobalEventBus.Bus.Pub(new UnregisterTicker(this));

    }

    void Update()
    {
        bool wasConnected = isConnected;

        if (lightStorage == null || (Vector3.Distance(transform.position, lightStorage.transform.position) > range))
        {
            var checkHit = Physics2D.OverlapCircleAll(transform.position, range, GameConstants.SolidLayer)
                .Where(e => e.gameObject.HasTag("LightStorage") || e.gameObject.HasTagInParent("LightStorage"))
                .FirstOrDefault();

            if (checkHit != null)
            {
                if (checkHit.gameObject.HasTag("LightStorage"))
                {
                    lightStorage = checkHit.gameObject.GetComponent<LightStorage>();
                    isConnected = true;
                }
                else if (checkHit.gameObject.HasTagInParent("LightStorage"))
                {
                    lightStorage = checkHit.gameObject.GetComponentInParent<LightStorage>();
                    isConnected = true;
                }
                else
                {
                    isConnected = false;
                }
            }
            else
            {
                isConnected = false;
            }
        }

        if (isConnected)
        {
            storageConnection.gameObject.SetActive(true);
            storageConnection.positionCount = 2;
            storageConnection.SetPosition(0, storageConnectorPoint.position);
            storageConnection.SetPosition(1, lightStorage.transform.position);
        }
        else
        {
            storageConnection.gameObject.SetActive(false);
            lightStorage = null;
        }
    }

    public void Tick(int tick)
    {
        if (!isConnected) return;

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
