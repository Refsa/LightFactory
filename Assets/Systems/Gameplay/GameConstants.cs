using System.Collections.Generic;
using UnityEngine;

public static class GameConstants
{
    public const float LightPacketSpeed = 0.25f;

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
}
