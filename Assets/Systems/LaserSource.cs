using System.Collections;
using System.Collections.Generic;
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

    int lastSendTick;
    List<LightPacket> packetsInTransport;

    int solidLayerMask;

    void Start()
    {
        solidLayerMask = 1 << LayerMask.NameToLayer("Solid");

        packetsInTransport = new List<LightPacket>();
        laserHits = new HashSet<GameObject>();
        vertices = new List<Vector3>();
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

        /* for (int i = 0; i < vertices.Count - 1; i++)
        {
            Vector2 dir = (vertices[i + 1] - vertices[i]);
            var hit = Physics2D.Raycast(vertices[i], dir.normalized, dir.magnitude, solidLayerMask);
            if (hit.collider != null && !laserHits.Contains(hit.collider.gameObject))
            {
                updateLaser = true;
                break;
            }
        } */

        // if (updateLaser || oldPosition != transform.position || oldRotation != transform.rotation)
        {
            TraceLaser();
            PushLineDrawerData();
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

        while (distanceLeft >= 0f && iterations < 10)
        {
            var hit = Physics2D.Raycast(currentPos + currentDir * 0.01f, currentDir, distanceLeft, solidLayerMask);
            if (hit.collider != null && hit.collider.gameObject != gameObject)
            {
                laserHits.Add(hit.collider.gameObject);
                currentPos = hit.point;
                vertices.Add(currentPos);
                if (hit.collider.gameObject.HasTag("Mirror"))
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
        if (tick - lastSendTick == sendRate)
        {
            var lightPacket = Pools.LightPacketPooler.Get()
                .Setup(LightPacketVisualPool.Instance.Get(), vertices[0], vertices[1], 1);

            lightPacket.Visual.GetComponent<LightPacketVisual>().SetColor(color);

            packetsInTransport.Add(lightPacket);

            lastSendTick = tick;
        }

        packetsInTransport.RemoveAll(e =>
            e.NextIndex >= vertices.Count || e.Next != vertices[e.NextIndex]
        );

        for (int i = packetsInTransport.Count - 1; i >= 0; i--)
        {
            TickLightPacket(packetsInTransport[i], i);
        }
    }

    void TickLightPacket(LightPacket lightPacket, int index)
    {
        var state = lightPacket.Tick();
        if (state != LightPacket.State.None)
        {
            if (lightPacket.NextIndex >= vertices.Count - 1)
            {
                Pools.LightPacketPooler.Free(lightPacket);
                LightPacketVisualPool.Instance.Free(lightPacket.Visual);

                packetsInTransport.RemoveAt(index);
            }
            else
            {
                int nextIndex = lightPacket.NextIndex + 1;
                lightPacket.SetNext(vertices[nextIndex], nextIndex);

                if (state == LightPacket.State.More)
                {
                    TickLightPacket(lightPacket, index);
                }
            }
        }
    }
}
