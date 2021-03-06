using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Refsa.EventBus;
using UnityEngine;

public class LaserSource : MonoBehaviour, ITicker
{
    static readonly Dictionary<Color, Gradient> LaserLineGradients = new Dictionary<Color, Gradient>();
    public static Gradient GetGradient(Color color, bool atMaxDistance)
    {
        if (!LaserLineGradients.TryGetValue(color, out var gradient))
        {
            gradient = MakeGradient(color, atMaxDistance);
        }

        return gradient;
    }
    static Gradient MakeGradient(Color color, bool atMaxDistance)
    {
        var gradient = new Gradient();

        gradient.colorKeys = new[]
        {
            new GradientColorKey(color, 0f),
            new GradientColorKey(color, 1f),
        };

        gradient.alphaKeys = new[]
        {
            new GradientAlphaKey(1f, 0f),
            new GradientAlphaKey(1f, 0.999f),
            new GradientAlphaKey(atMaxDistance ? 0f : 1f, 1f),
        };

        return gradient;
    }
    public static void ClearGradients()
    {
        if (LaserLineGradients != null) LaserLineGradients.Clear();
    }

    [SerializeField] Transform source;
    [SerializeField] LineRenderer laserRenderer;
    [SerializeField] float maxDistance = 20f;
    [SerializeField, Tooltip("Ticks between each light packet")] int sendRate = 4;
    [SerializeField] Color color = Color.white;

    new Camera camera;

    Vector3 oldPosition;
    Quaternion oldRotation;

    List<Vector3> vertices;
    HashSet<GameObject> laserHits;
    Dictionary<ILaserCollector, Connection> connections;
    float distanceNorm;

    int defaultSendRate;

    int lastSendTick;
    List<LightPacket> packetsInTransport;

    int solidLayerMask;

    public int TickerPriority => TickerPriorities.LASER_SOURCE;
    public Color Color => color;

    void Start()
    {
        defaultSendRate = sendRate;

        solidLayerMask = GameConstants.SolidLayer;

        packetsInTransport = new List<LightPacket>();
        laserHits = new HashSet<GameObject>();
        vertices = new List<Vector3>();
        connections = new Dictionary<ILaserCollector, Connection>();
        camera = Camera.main;

        oldPosition = transform.position;
        oldRotation = transform.rotation;

        // vertices.Add(source.position);
        // vertices.Add(vertices[0] + source.up * 5f);
        // vertices.Add(vertices[1] + source.right * 5f);

        TraceLaser();
        PushLineDrawerData();

        OnEnable();
    }

    void OnEnable()
    {
        GlobalEventBus.Bus.Pub(new RegisterTicker(this));

        laserHits = new HashSet<GameObject>();
        connections = new Dictionary<ILaserCollector, Connection>();
    }

    void OnDisable()
    {
        GlobalEventBus.Bus.Pub(new UnregisterTicker(this));
    }

    void Update()
    {
        bool updateLaser = false;

        if (updateLaser || oldPosition != transform.position || oldRotation != transform.rotation)
        {
            ClearLightPackets();
        }

        {
            TraceLaser();
            PushLineDrawerData();
            CorrectPackets();
        }

        oldPosition = transform.position;
        oldRotation = transform.rotation;
    }

    void TraceLaser()
    {
        laserHits.Clear();
        vertices.Clear();

        Vector2 currentPos = source.position;
        Vector2 currentDir = source.up;
        vertices.Add(currentPos);

        float distanceLeft = maxDistance;
        int iterations = 0;
        bool blocked = false;

        while (distanceLeft > 0f && iterations < 10)
        {
            var hit = Physics2D.Raycast(currentPos + currentDir * 0.01f, currentDir, distanceLeft, solidLayerMask);
            var otherGO = hit.collider == null ? null : hit.collider.gameObject;

            if (otherGO != null && otherGO != gameObject)
            {
                laserHits.Add(otherGO);
                currentPos = hit.point;
                vertices.Add(currentPos);

                if (otherGO.HasTag("Mirror"))
                {
                    if (hit.collider.GetComponentInParent<ILaserReflector>().TryReflect(currentDir, hit.normal, out var reflected))
                    {
                        currentDir = Vector2.Reflect(currentDir, hit.normal);
                        distanceLeft -= hit.distance;
                    }
                    else
                    {
                        blocked = true;
                        break;
                    }
                }
                else if (otherGO.HasTag("LaserCollector"))
                {
                    var collector = otherGO.GetComponentInParent<ILaserCollector>();
                    if (!connections.TryGetValue(collector, out var conn))
                    {
                        conn = new Connection(this, collector);
                        connections.Add(collector, conn);
                        collector.NotifyConnected(conn);
                    }
                    conn.LastConnectionTime = Time.frameCount;
                    blocked = true;
                    break;
                }
                else
                {
                    blocked = true;
                    break;
                }
            }
            else
            {
                break;
            }
            iterations++;
        }

        for (int i = connections.Count - 1; i >= 0; i--)
        {
            var kvp = connections.ElementAt(i);
            if (kvp.Value.LastConnectionTime != Time.frameCount)
            {
                connections.Remove(kvp.Key);
                kvp.Key.NotifyDisconnected(kvp.Value);
            }
        }

        if (!blocked)
        {
            vertices.Add(currentPos + currentDir * distanceLeft);
            distanceNorm = 0f;
        }
        else
        {
            distanceNorm = distanceLeft;
        }

    }

    void CorrectPackets()
    {
        for (int i = packetsInTransport.Count - 1; i >= 0; i--)
        {
            var packet = packetsInTransport[i];

            if (packet.NextIndex >= vertices.Count)
            {
                FreeLightPacket(packet);
                packetsInTransport.Remove(packet);
            }
            else if (packet.Next != vertices[packet.NextIndex])
            {
                /* packet.SetNext(vertices[packet.NextIndex], packet.NextIndex);
                Vector2 pos = MathHelpersGetClosestPointOnLineSegment(vertices[packet.NextIndex - 1], packet.Next, packet.Position);
                packet.SetPosition(pos); */
                FreeLightPacket(packet);
                packetsInTransport.Remove(packet);
            }
            else
            {
                packet.Update();
            }
        }
    }

    void PushLineDrawerData()
    {
        laserRenderer.colorGradient = GetGradient(color, distanceNorm < 0.5f);
        laserRenderer.positionCount = vertices.Count;
        for (int i = 0; i < vertices.Count; i++)
        {
            laserRenderer.SetPosition(i, vertices[i]);
        }
    }

    public void Tick(int tick)
    {
        if (sendRate != -1 && tick - lastSendTick >= sendRate)
        {
            if (connections.Count > 0)
            {
                NewPacket();
            }
            lastSendTick = tick;
        }

        for (int i = packetsInTransport.Count - 1; i >= 0; i--)
        {
            var packet = packetsInTransport[i];
            TickLightPacket(packet);
        }
    }

    void TickLightPacket(LightPacket lightPacket)
    {
        var state = lightPacket.Tick();
        Debug.Log(state);
        if (state != LightPacket.State.Move)
        {
            if (lightPacket.NextIndex >= vertices.Count - 1)
            {
                foreach (var collector in connections.Values)
                {
                    collector.Dest.Notify(color, lightPacket);
                }

                FreeLightPacket(lightPacket);
            }
            else
            {
                int nextIndex = lightPacket.NextIndex + 1;
                lightPacket.SetNext(vertices[lightPacket.NextIndex], nextIndex);

                if (state == LightPacket.State.More)
                {
                    TickLightPacket(lightPacket);
                }
            }
        }
    }

    public void NewPacket(LightLevel lightLevel = LightLevel.One)
    {
        var lightPacket = Pools.LightPacketPooler.Get()
            .Setup(
                LightPacketVisualPool.Instance.Get(),
                vertices[0],
                vertices[1],
                1);

        var visual = lightPacket.Visual.GetComponent<LightPacketVisual>();
        visual.SetColor(color);
        visual.SetLevel(lightLevel);

        packetsInTransport.Add(lightPacket);
    }

    void FreeLightPacket(LightPacket lightPacket)
    {
        packetsInTransport.Remove(lightPacket);

        LightPacketVisualPool.Instance.Free(lightPacket.Visual);
        lightPacket.Clear();
        Pools.LightPacketPooler.Free(lightPacket);
    }

    void ClearLightPackets()
    {
        for (int j = packetsInTransport.Count - 1; j >= 0; j--)
        {
            FreeLightPacket(packetsInTransport[j]);
        }
        packetsInTransport.Clear();
    }

    public void SetColor(Color color)
    {
        this.color = color;
    }

    public void SetRange(float range)
    {
        this.maxDistance = range;
    }

    public void SetRate(int rate)
    {
        this.sendRate = rate;
    }

    public void ResetRate()
    {
        this.sendRate = defaultSendRate;
    }

    public void Enable()
    {
        laserRenderer.enabled = true;
        enabled = true;
    }

    public void Disable()
    {
        laserRenderer.enabled = false;
        enabled = false;
        ClearLightPackets();
    }
}
