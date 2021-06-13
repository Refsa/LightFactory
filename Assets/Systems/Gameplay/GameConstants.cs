using System.Collections.Generic;
using UnityEngine;

public enum Colors
{
    Red = 0,
    Green,
    Blue,
    Cyan,
    Magenta,
    Yellow,
    White
}

public enum LightLevel
{
    One = 0,
    Two,
    Three,
    Four
}

public static class GameConstants
{
    public const int LightPacketOne = 0;
    public const int LightPacketTwo = 1;
    public const int LightPacketThree = 2;
    public const int LightPacketFour = 3;

    public const float LightPacketSpeed = 0.25f;

    public const float GridMajorSnap = 0.5f;
    public const float GridMinorSnap = 0.1f;

    public const float RotationMajorSnap = 11.25f;
    public const float RotationMinorSnap = 2.8125f;

    public static int ColorToID(Color color)
    {
        bool hasRed = color.r > 0f;
        bool hasGreen = color.g > 0f;
        bool hasBlue = color.b > 0f;

        if (hasRed)
        {
            if (hasBlue)
            {
                if (hasGreen)
                {
                    return 6;
                }
                else
                {
                    return 4;
                }
            }
            else if (hasGreen)
            {
                return 3;
            }
            else
            {
                return 0;
            }
        }
        else if (hasBlue)
        {
            if (hasGreen)
            {
                return 5;
            }
            else
            {
                return 1;
            }
        }
        else if (hasGreen)
        {
            return 2;
        }

        return -1;
    }

    public static Color IDToColor(int id)
    {
        if (id > 5) return Color.white;

        switch (id)
        {
            case 0: return Color.red;
            case 1: return Color.blue;
            case 2: return Color.green;
            case 3: return Color.yellow;
            case 4: return Color.magenta;
            case 5: return Color.cyan;
        }

        return Color.white;
    }

    public static Color CombineColor(in Color a, in Color b)
    {
        if (a == Color.red && b == Color.green || a == Color.green && b == Color.red)
        {
            return Color.yellow;
        }
        if (a == Color.red && b == Color.blue || a == Color.blue && b == Color.red)
        {
            return Color.magenta;
        }
        if (a == Color.blue && b == Color.green || a == Color.green && b == Color.blue)
        {
            return Color.cyan;
        }

        throw new System.ArgumentException($"Colors {a} and {b} cant be combined");
    }
}
