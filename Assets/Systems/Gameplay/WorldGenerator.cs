using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
    [SerializeField] GameObject resourceFieldGameobject;
    [SerializeField] float worldSize = 512f;
    [SerializeField] float spread = 10f;

    PoissonDiscSampler poissonDiscSampler;
    HashSet<Vector2> seen;

    void Start()
    {
        seen = new HashSet<Vector2>();
        poissonDiscSampler = new PoissonDiscSampler(worldSize, worldSize, spread);
        Generate();
    }

    void Generate()
    {
        seen.Clear();

        foreach (var sample in poissonDiscSampler.Samples())
        {
            if (!seen.Add(sample))
            {
                continue;
            }

            var go = GameObject.Instantiate(resourceFieldGameobject, transform);
            go.transform.position = sample - (Vector2.one * worldSize * 0.5f);
            go.transform.position = go.transform.position.Snap(GameConstants.GridMinorSnap);
        }
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Color inner = new Color(1,1,1,0.05f);
        Color outer = Color.white;
        Rect world = new Rect(Vector2.zero, Vector2.one * worldSize);
        world.center -= (world.size * 0.5f);

        UnityEditor.Handles.DrawSolidRectangleWithOutline(world, inner, outer);
    }
#endif
}
