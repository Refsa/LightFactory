using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleReflector : MonoBehaviour, ILaserReflector
{
    public bool TryReflect(Vector2 dir, Vector2 normal, out Vector2 reflected)
    {
        reflected = Vector2.Reflect(dir, normal);
        return true;
    }
}
