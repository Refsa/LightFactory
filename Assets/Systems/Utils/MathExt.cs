using UnityEngine;

public static class MathExt
{
    public static Vector3 ToVector3(in this Vector2 self)
    {
        return new Vector3(self.x, self.y, 0f);
    }
}