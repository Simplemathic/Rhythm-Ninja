using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Note : IComparable<Note>
{
    public int ID;
    public double positionInBeats;          // position measured in beats starting from the beat defined by the offset in Music
    public double positionInTime;           // time variant of positionInBeats
    public InputHandler.Input key;          // the key which should be pressed on this note
    public Status status;                   // false, if the player already did hit any button on this note
    public double speed;                    // the speed (per sec) which the note comes towards the player with
    public Position positionInSpace;
    private Conductor conductor;

    public delegate void OnMissedDelegate();
    public OnMissedDelegate onMissed;

    public enum Position
    {
        RIGHT,
        LEFT,
        MIDDLE
    }

    public enum Score
    {
        CRITICAL,       // button pressed very accurately
        NORMAL,         // button pressed within pointThreshold
        BARELY,         // button pressed within hitThreshold
        EARLY,          // button pressed before note reaching threshold
        LATE,           // button pressed after note leaving threshold      - never should be used
        WRONG_KEY,      // wrong button pressed while note was in threshold
        NONE            // note is already evaluated (ACTIVATED / RUINED)   - never should be used
    }

    public enum Status
    {
        RUINABLE,       // note is inside the outer threshold, but outside the inner threshold
        ACTIVATABLE,    // note is inside the threshold
        ACTIVATED,      // the correct button was pressed while note was inside the threshold
        ACTIVATING,     /** TODO **/
        RUINED,         // wrong button was pressed while note was inside the threshold, or button was pressed while note was inside the outer threshold
        GOING,          // note left the threshold before getting evaluated
        COMING          // not has not reached the threshold yet
    }

    public Note(double positionInBeats, InputHandler.Input keyCode = LevelInformation.defaultKeyCode, Position position = Position.RIGHT, double speed = LevelInformation.defaultSpeed)
    {
        ID = -1;
        key = keyCode;
        this.positionInBeats = positionInBeats;
        this.speed = speed;
        status = Status.COMING;

        if (key == InputHandler.Input.BASS)
        {
            if (position != Position.MIDDLE)
            {
                Debug.LogWarning("A BASS not can only be in position MIDDLE. Changed position from " + position + " to MIDDLE!");
            }
            positionInSpace = Position.MIDDLE;
        }
        else if (position == Position.MIDDLE)
        {
            Debug.LogWarning("A note which is not a BASS type, can not be in MIDDLE position. Position is changed from " + position + " to RIGHT!");
            positionInSpace = Position.RIGHT;
        }
        else
        {
            positionInSpace = position;
        }
    }

    public void calculatePositionInTime(double bpm)
    {
        double secPerBeat = 60 / bpm;
        positionInTime = positionInBeats * secPerBeat + Settings.inputOffset;
    }

    public void setConductor(Conductor conductor)
    {
        this.conductor = conductor;
    }

    public double getAccuracy(double time)
    {
        double accuracy = time - positionInTime;
        Debug.Log("Accuracy = " + accuracy);
        return accuracy;
    }

    /* 
     * Activates the note effect based on the status field
     * The result is a score which is calculated using global constants
     * representing the note's affordable distances from the player for
     * each type of score value 
     */
    public virtual Score Hit(InputHandler.Input key, double songTime, double bpm)
    {
        bool correctKey = this.key == key || this.key == InputHandler.Input.ANY;

        switch (status)
        {
            case Status.ACTIVATABLE:
                if (correctKey)
                {
                    status = Status.ACTIVATED;
                }
                else
                {
                    status = Status.RUINED;
                    return Score.WRONG_KEY;
                }

                double hitThreshold = LevelInformation.hitThreshold;
                double pointThreshold = LevelInformation.pointThreshold;
                double criticalThreshold = LevelInformation.criticalThreshold;
                double distance = songTime - positionInTime;

                if (Math.Abs(distance) <= criticalThreshold)
                {
                    return Score.CRITICAL;
                }
                else if (Math.Abs(distance) <= pointThreshold)
                {
                    return Score.NORMAL;
                }
                else if (Math.Abs(distance) <= hitThreshold)
                {
                    return Score.BARELY;
                }
                else
                {
                    Debug.LogWarning("Note[" + positionInBeats + "] has ACTIVATABLE status, but is out of hit threshold!");
                    return Score.NONE;
                }
            case Status.ACTIVATING:
                /** TODO - no exact definition yet **/
                break;
            case Status.ACTIVATED:
                return Score.NONE;
            case Status.RUINED:
                return Score.NONE;
            case Status.GOING:
                return Score.LATE;
            case Status.COMING:
                return Score.EARLY;
            case Status.RUINABLE:
                status = Status.RUINED;
                if (correctKey)
                    return Score.EARLY;
                else
                    return Score.WRONG_KEY;
            default:
                break;
        }
        return Score.NONE;
    }

    /* 
     * Updates the status field, based on global constants
     * These constants represent the note's affordable distances from
     * the player in order to recieve points 
     */
    public void Update(double songTime, double bpm)
    {
        double distance = songTime - positionInTime;
        double hitThreshold = LevelInformation.hitThreshold;
        double ruinableThreshold = LevelInformation.ruinThreshold;

        if (Math.Abs(distance) <= hitThreshold)
        {
            if (status == Status.COMING || status == Status.RUINABLE)
                status = Status.ACTIVATABLE;
        }
        else if (distance > hitThreshold)
        {
            if (status == Status.ACTIVATABLE)
            {
                status = Status.GOING;
                /* Alert the conductor */
                conductor.onMiss(ID);
            }
        }
        else if (-ruinableThreshold <= distance && distance < -hitThreshold)
        {
            if (status == Status.COMING)
            {
                status = Status.RUINABLE;
            }
        }
        else if (-ruinableThreshold >  distance)
        {
            if (status != Status.COMING)
            {
                Debug.LogWarning("Note[" + positionInBeats + "] has " + status + " status, but should be COMING, as it is still ahead in time.\n\tdt = " + LevelInformation.dt);
                status = Status.COMING;
            }
        }
    }

    public int CompareTo(Note obj)
    {
        if (positionInTime < obj.positionInTime)
            return -1;
        else if (positionInTime > obj.positionInTime)
            return 1;
        return 0;
    }
}
