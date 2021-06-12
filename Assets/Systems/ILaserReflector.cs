using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILaserReflector
{
    bool TryReflect(Vector2 dir, Vector2 normal, out Vector2 reflected);
}
