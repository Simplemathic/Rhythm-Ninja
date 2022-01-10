using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class Settings
{
    public const double defInputOffset = 0.0;
    public const float defVolume = 1.0f;

    public static double inputOffset;
    public static float volume;

    public static float normalizeVolume()
    {
        return (float)Math.Pow(volume, 2);
    }
}
