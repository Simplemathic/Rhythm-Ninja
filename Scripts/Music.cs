using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Music
{
    /* basic info */
    public string title;
    public string artist;
    public double bpm;
    public AudioClip musicSource;
    public List<Note> notes;


    /* 
     * OFFSETS
     *   startOffset... - extra time before starting the audio file
     *   audioOffset... - offset found inside the audio file
     *   endOffset...   - extra time after the last note (in notes) crosses the player
     *   
     *   ...inSec       - measured in seconds
     *   ...inBeats     - measured in beats (bpm)   
     */
    public double startOffsetTime;
    public double startOffsetBeats;
    public double audioOffsetTime;
    public double endOffsetTime;
    public double endOffsetBeats;


    /* 
     * LENGTHS 
     *   lengthInBeats - the last note's positionInBeats value, which does NOT 
     *                   represent the actual length of the song measured in beats
     *   lengthInTime  - the actual length of the music determined by the last
     *                   note's relativePositionInTime value plus the end offset, 
     *                   this is measured from the beginning of the audio file
     */
    public double lengthInBeats;
    public double lengthInTime;


    public Music(string title, string artist, double bpm, double audioOffsetSec = 0, double startOffsetSec = 0, double endOffsetSec = 0)
    {
        this.notes = new List<Note>();
        this.title = title;
        this.artist = artist;
        this.bpm = bpm;
        this.audioOffsetTime = audioOffsetSec;
        this.startOffsetTime = startOffsetSec;
        this.endOffsetTime = endOffsetSec;
    }

    public void setNotes(List<Note> _notes, Conductor conductor)
    {
        notes = _notes;
        setConductorToNotes(conductor);
        setNotePositionsInTime();
        sortNotes();
        setLength();
        setNoteIDs();
    }

    private void sortNotes()
    {
        notes.Sort();
    }

    private void setConductorToNotes(Conductor conductor)
    {
        for (int i = 0; i < notes.Count; i++)
            notes[i].setConductor(conductor);
    }

    /* Only call if notes is ordered! */
    private void setNoteIDs()
    {
        for (int i = 0; i < notes.Count; i++)
            notes[i].ID = i;
    }

    /* Only call if notes is ordered! */
    private void setLength()
    {
        if (notes.Count == 0)
        {
            lengthInBeats = 0;
            lengthInTime =  0;
            Debug.Log("Music has no notes, cannot set length!");
            return;
        }
        lengthInBeats = notes[notes.Count - 1].positionInBeats;
        lengthInTime  = notes[notes.Count - 1].positionInTime + endOffsetTime;
    }

    public void setMusic(AudioClip music)
    {
        musicSource = music;
    }

    public void UpdateNotes(double songPosition, double bpm)
    {
        for (int i = 0; i < notes.Count; i++)
            notes[i].Update(songPosition, bpm);
    }

    private void setNotePositionsInTime()
    {
        for (int i = 0; i < notes.Count; i++)
            notes[i].calculatePositionInTime(bpm);
    }

    public void printNoteStatuses()
    {
        string text = "[";
        for (int i = 0; i < notes.Count; i++)
        {
            text += notes[i].status;
            if (i + 1 < notes.Count) text += ", ";
        }
        text += "]";
        Debug.Log(text);
    }
}
