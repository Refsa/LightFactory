using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Refsa.EventBus;
using UnityEngine;

public class LaserSource : MonoBehaviour, ITicker
{
    static readonly Dictionary<Color, Gradient> LaserLineGradients = new Dictionary<Color, Gradient>();
    static Gradient GetGradient(Color color)
    {
        if (!LaserLineGradients.TryGetValue(color, out var gradient))
        {
            gradient = MakeGradient(color);
        }

        return gradient;
    }
    static Gradient MakeGradient(Color color)
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
            new GradientAlphaKey(1f, 0.8f),
            new GradientAlphaKey(0f, 1f),
        };

        return gradient;
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
    HashSet<LaserCollector> collectors;
    HashSet<Vector3> oldVertices;

    int lastSendTick;
    List<LightPacket> packetsInTransport;

    int solidLayerMask;

    public int TickerPriority => TickerPriorities.LASER_SOURCE;

    void Start()
    {
        solidLayerMask = 1 << LayerMask.NameToLayer("Solid");

        packetsInTransport = new List<LightPacket>();
        laserHits = new HashSet<GameObject>();
        vertices = new List<Vector3>();
        oldVertices = new HashSet<Vector3>();
        collectors = new HashSet<LaserCollector>();
        camera = Camera.main;

        oldPosition = transform.position;
        oldRotation = transform.rotation;

        // vertices.Add(source.position);
        // vertices.Add(vertices[0] + source.up * 5f);
        // vertices.Add(vertices[1] + source.right * 5f);

        TraceLaser();
        PushLineDrawerData();
    }

    void OnEnable()
    {
        GlobalEventBus.Bus.Pub(new RegisterTicker(this));
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
        oldVertices.Clear();
        foreach (var vert in vertices.Skip(1).Take(vertices.Count - 2)) oldVertices.Add(vert);

        collectors.Clear();
        laserHits.Clear();
        vertices.Clear();

        Vector2 currentPos = source.position;
        Vector2 currentDir = source.up;
        vertices.Add(currentPos);

        float distanceLeft = maxDistance;
        int iterations = 0;
        bool blocked = false;

        while (distanceLeft >= 0f && iterations < 10)
        {
            var hit = Physics2D.Raycast(currentPos + currentDir * 0.01f, currentDir, distanceLeft, solidLayerMask);
            var otherGO = hit.collider == null ? null : hit.collider.gameObject;

            if (otherGO != null && otherGO != gameObject)
            {
                laserHits.Add(otherGO);
                currentPos = hit.point;
                vertices.Add(currentPos);

                if (otherGO.HasTagInParent("Mirror"))
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
                else if (otherGO.HasTagInParent("LaserCollector"))
                {
                    collectors.Add(otherGO.GetComponentInParent<LaserCollector>());
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

        if (!blocked)
        {
            vertices.Add(currentPos + currentDir * distanceLeft);
        }

        /* for (int i = 1; i < vertices.Count - 2; i++)
        {
            if (!oldVertices.Contains(vertices[i]))
            {
                ClearLightPackets();
                break;
            }
        } */
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
                packet.SetNext(vertices[packet.NextIndex], packet.NextIndex);
                Vector2 pos = GetClosestPointOnLineSegment(vertices[packet.NextIndex - 1], packet.Next, packet.Position);
                packet.SetPosition(pos);
            }
        }
    }

    void PushLineDrawerData()
    {
        laserRenderer.colorGradient = GetGradient(color);
        laserRenderer.positionCount = vertices.Count;
        for (int i = 0; i < vertices.Count; i++)
        {
            laserRenderer.SetPosition(i, vertices[i]);
        }
    }

    public void Tick(int tick)
    {
        if (tick - lastSendTick >= sendRate)
        {
            var lightPacket = Pools.LightPacketPooler.Get()
                .Setup(LightPacketVisualPool.Instance.Get(), vertices[0], vertices[1], 1);

            lightPacket.Visual.GetComponent<LightPacketVisual>().SetColor(color);

            packetsInTransport.Add(lightPacket);

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
        if (state != LightPacket.State.None)
        {
            if (lightPacket.NextIndex >= vertices.Count - 1)
            {
                FreeLightPacket(lightPacket);

                foreach (var collector in collectors)
                {
                    collector.Notify(color);
                }
            }
            else
            {
                int nextIndex = lightPacket.NextIndex + 1;
                lightPacket.SetNext(vertices[nextIndex], nextIndex);

                if (state == LightPacket.State.More)
                {
                    TickLightPacket(lightPacket);
                }
            }
        }
    }

    void FreeLightPacket(LightPacket lightPacket)
    {
        Pools.LightPacketPooler.Free(lightPacket);
        LightPacketVisualPool.Instance.Free(lightPacket.Visual);
        packetsInTransport.Remove(lightPacket);
    }

    void ClearLightPackets()
    {
        for (int j = packetsInTransport.Count - 1; j >= 0; j--)
        {
            FreeLightPacket(packetsInTransport[j]);
        }
        packetsInTransport.Clear();
    }

    public static Vector2 GetClosestPointOnLineSegment(Vector2 A, Vector2 B, Vector2 P)
    {
        Vector2 AP = P - A;
        Vector2 AB = B - A;

        float magnitudeAB = AB.sqrMagnitude;
        float ABAPproduct = Vector2.Dot(AP, AB);
        float distance = ABAPproduct / magnitudeAB;

        if (distance < 0)
        {
            return A;

        }
        else if (distance > 1)
        {
            return B;
        }
        else
        {
            return A + AB * distance;
        }
    }
}
