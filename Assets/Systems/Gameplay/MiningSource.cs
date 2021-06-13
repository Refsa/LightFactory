using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiningSource : MonoBehaviour, ITicker
{
    [SerializeField] GameObject resourcePrefab;
    [SerializeField] Vector2Int resourceSpots = new Vector2Int(1, 8);

    MiningResource[] resources;

    public int TickerPriority => TickerPriorities.LASER_SOURCE;

    void Awake()
    {
        Randomize();
    }

    public void Tick(int tick)
    {

    }

    public void Randomize()
    {
        Color color = GameConstants.IDToColor(Random.Range(0, 3));

        int resourceCount = Random.Range(resourceSpots.x, resourceSpots.y);
        resources = new MiningResource[resourceCount];

        float theta = 0f;
        float step = 360f / resourceCount;

        for (int i = 0; i < resourceCount; i++)
        {
            resources[i] = GameObject.Instantiate(resourcePrefab, transform).GetComponent<MiningResource>();

            resources[i].transform.up = Quaternion.Euler(0f, 0f, theta) * Vector3.up;
            resources[i].transform.position += resources[i].transform.up * 0.25f;

            resources[i].Randomize(color);

            theta += step;
        }
    }
}
