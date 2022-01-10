using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Contains core information about the selected level. These information include:
 * - statistics
 * - score
 * - time variables
 * - global constants
 */
public static class LevelInformation
{
    public const double defaultSpeed = 6.0;
    public const double playerVerticalPosition = -3.0;
    public const double playerDistanceFromWall = 1.0;
    public const float wallDistanceFromCenter = 4.0f;
    public const float centerHorizontalPosition = 0.0f;
    public const InputHandler.Input defaultKeyCode = InputHandler.Input.ANY;

    public static int? songID = null;
    public static bool survived = true;
    public static double dt = 0.0;
    public static double speed = 6.0;
    public static double secPerBeat = 1.0;
    public static int criticalStrike = 0;
    public static Note.Position playerPosition = Note.Position.RIGHT;
    public static double averageAccuracy = 0;



    /* The note is able to be hit within this timeframe, but no points granted */
    public const double hitThreshold = 0.3;

    /* The note is able to be hit and points can be earned */
    public const double pointThreshold = 0.11;

    /* The note is able to be hit for extra points within this timeframe */
    public const double criticalThreshold = 0.018;

    /* The note is able to be hit, but negative points will be granted if doing so */
    public const double ruinThreshold = 0.6;



    /* point values */
    public const int pointsForCritical = 100;
    public const int pointsForHit = 50;
    public const int pointsForBarely = 25;
    public const int pointsForMiss = 0;
    public const int pointStepForCriticalStrike = 10;



    /* Level data for summary */
    public static string levelTitle = "";
    public static string artist = "";
    public static int score = 0;
    public static int criticalCount = 0;
    public static int hitCount = 0;
    public static int barelyCount = 0;
    public static int missCount = 0;



    public static void update(string _title = "", string _artist = "")
    {
        levelTitle = _title;
        artist = _artist;
        score = 0;
        criticalCount = 0;
        hitCount = 0;
        barelyCount = 0;
        missCount = 0;
        averageAccuracy = 0;
    }

    public static void handleScore(Note.Score noteScore)
    {
        if (noteScore != Note.Score.CRITICAL)
            criticalStrike = 0;

        switch (noteScore)
        {
            case Note.Score.CRITICAL:
                criticalCount++;
                criticalStrike++;
                score += pointsForCritical + (criticalStrike - 1) * pointStepForCriticalStrike;
                break;
            case Note.Score.NORMAL:
                hitCount++;
                score += pointsForHit;
                break;
            case Note.Score.BARELY:
                barelyCount++;
                score += pointsForBarely;
                break;
            case Note.Score.EARLY:
            case Note.Score.LATE:
            case Note.Score.WRONG_KEY:
            case Note.Score.NONE:
            default:
                missCount++;
                score += pointsForMiss;
                break;
        }
    }
}
