using UnityEngine;

public static class MathExt
{
    public static Vector3 ToVector3(in this Vector2 self)
    {
        return new Vector3(self.x, self.y, 0f);
    }

    public static Vector2 Snap(this Vector2 self, float snap)
    {
        self.x = Mathf.Round(self.x / snap) * snap;
        self.y = Mathf.Round(self.y / snap) * snap;
        return self;
    }

    public static Vector3 Snap(this Vector3 self, float snap)
    {
        self.x = Mathf.Round(self.x / snap) * snap;
        self.y = Mathf.Round(self.y / snap) * snap;
        self.z = Mathf.Round(self.z / snap) * snap;
        return self;
    }
}