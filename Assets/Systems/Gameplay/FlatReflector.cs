using UnityEngine;

public class FlatReflector : MonoBehaviour, ILaserReflector
{
    [SerializeField] Transform reflector;

    public bool TryReflect(Vector2 dir, Vector2 normal, out Vector2 reflected)
    {
        float forwardDot = Vector2.Dot(reflector.right, dir);

        if (forwardDot >= 0f)
        {
            reflected = default;
            return false;
        }

        reflected = Vector2.Reflect(dir, normal);
        return true;
    }
}