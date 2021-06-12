using UnityEngine;

public class MiningResource : MonoBehaviour
{
    [SerializeField] LaserSource laserSource;

    [SerializeField] Vector2Int resourceRange = new Vector2Int(1 << 10, 1 << 16);

    int currentResources;
    Color currentColor;

    public void Randomize(Color color)
    {
        currentResources = Random.Range(resourceRange.x, resourceRange.y);
        currentColor = color;

        laserSource.SetColor(color);
        laserSource.SetRange(1f);
    }

    public void Drain(int count = 1)
    {
        currentResources -= count;
    }
}