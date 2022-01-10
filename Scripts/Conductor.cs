using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

/*
 * The most important class, clearly disobeys some clean code laws and principles (like SRP and ISP)
 * - Plays the audio file
 * - Manages the time variables representing the current position inside the audio file
 * - Moves note blocks
 * - Contains the logic that decides which note the player has hit
 * - etc.
 */
public class Conductor : MonoBehaviour
{
    public Player player;
    public AudioSource musicPlayer;
    public Text tCurrentBeat;
    public Text tScore;
    public GameManager gm;

    [Range(4, 1000)]
    public int targetFPS = 1000;

    [HideInInspector] public Music music;
    [HideInInspector] public double secPerBeat;                     // time in sec for one beat
    [HideInInspector] public double songPosition;                   // playhead position (sec) without smoothing, used for input
    [HideInInspector] public double songPositionInBeats;            // playhead position in beats
    [HideInInspector] public double lastSongPositionInBeats;        // playhead position in beats in the previous frame
    [HideInInspector] public double dspSongStartTime;               // playhead position in the previous frame in beats
    [HideInInspector] public double lastDspTime;                    // raw playhead position in the previous frame
    [HideInInspector] public bool gotNewDspTime = true;
    [HideInInspector] public bool playing = false;
    private List<GameObject> gameObjects;
    private List<double> accuracyStatistic;

    private int nextNoteIndex = 0;
    private const double spawnPosition = 6.0;

    void Awake()
    {
        gameObjects = new List<GameObject>();
    }

    public void Play(Music music)
    {
        Application.targetFrameRate = targetFPS;

        /* gaining data about music */
        this.music = music;
        secPerBeat = 60.0 / music.bpm;
        LevelInformation.secPerBeat = secPerBeat;

        /* resetting every variable for a smooth start */
        accuracyStatistic = new List<double>();
        songPosition = -1.0 * (music.audioOffsetTime + music.startOffsetTime);  // song starts from offset
        songPositionInBeats = songPosition / secPerBeat;
        player.setHealthToMax();

        /* starting the music */
        musicPlayer.clip = music.musicSource;
        dspSongStartTime = AudioSettings.dspTime + music.startOffsetTime;
        lastDspTime = AudioSettings.dspTime;
        musicPlayer.volume = Settings.normalizeVolume();
        musicPlayer.PlayScheduled(dspSongStartTime);
        playing = true;
    }

    void Update()
    {
        // return if no music is playing
        if (!playing) return;

        // ending the level
        if (songPosition > music.lengthInTime)
        {
            gm.onMusicEnd();
        }

        /* filling missing dspTime samples with deltaTimes */
        var dspTime = AudioSettings.dspTime;
        double increment = 0.0;
        if (dspTime != lastDspTime)
        {
            double dspFromStart = dspTime - dspSongStartTime;
            increment = dspFromStart - (songPosition + music.audioOffsetTime);
            gotNewDspTime = true;
        }
        else
        {
            increment = 0;// Time.unscaledDeltaTime / 2;
            gotNewDspTime = false;
        }

        /* increasing playhead position */
        songPosition += increment;
        LevelInformation.dt = increment;
        if (increment < 0) Debug.LogWarning("Negative delta time! (" + increment +")"); 

        /* updating notes */
        songPositionInBeats = songPosition / secPerBeat;
        music.UpdateNotes(songPosition, music.bpm);

        /* displaying information */
        int quarter = ((int)(songPositionInBeats * 100.0f) % 100) / 25;
        tCurrentBeat.text = "" + ((int)songPositionInBeats) + ":" + (quarter + 1);
        tScore.text = LevelInformation.score.ToString();

        /* spawning new notes */
        if (nextNoteIndex < music.notes.Count)
        {
            double t = (music.notes[nextNoteIndex].positionInBeats - songPositionInBeats) * secPerBeat + Settings.inputOffset;
            double s = (spawnPosition - LevelInformation.playerVerticalPosition);
            double v = LevelInformation.speed;
            while (t <= s / v)
            {
                /* The note may has already passed spawnPosition, we should calculate it's exact location to spawn at */
                double spawnDistance = t * v + LevelInformation.playerVerticalPosition;
                gameObjects.Add(ObjectPooler.Instance.SpawnFromPool(music.notes[nextNoteIndex], (float)spawnDistance));

                nextNoteIndex++;
                if (nextNoteIndex >= music.notes.Count) break;

                t = (music.notes[nextNoteIndex].positionInBeats - songPositionInBeats) * secPerBeat + Settings.inputOffset;
            }
        }
        else
        {
            Debug.Log("Reached end of music data");
        }

        /* Coloring notes based on their status - ONLY FOR DEBUGGING */
        for (int i = 0; i < gameObjects.Count; i++)
        {
            Color color = new Color(1, 1, 1);
            switch (music.notes[i].status)
            {
                case Note.Status.ACTIVATABLE:
                    break;
                case Note.Status.RUINABLE:
                    break;
                case Note.Status.ACTIVATED:
                    color = new Color(0.0f, 1.0f, 0.0f);
                    break;
                case Note.Status.COMING:
                    break;
                case Note.Status.GOING:
                    color = new Color(0.4f, 0.4f, 0.4f);
                    break;
                case Note.Status.RUINED:
                    color = new Color(1.0f, 0.0f, 0.0f);
                    break;
                default:
                    break;
            }
            gameObjects[i].GetComponent<Renderer>().material.color = color;
        }

        player.gainHealth((int)songPositionInBeats - (int)lastSongPositionInBeats);

        // store previous values for future comparison
        lastDspTime = dspTime;
        lastSongPositionInBeats = songPositionInBeats;
    }

    /* 
     * Returns the first Note in music.notes that is able to be hit
     * music.notes has to be ordered by time in order to work properly! 
     */
    public Note findTargetNote(InputHandler.Input inputType)
    {
        if (music == null)
        {
            Debug.LogWarning("No music loaded, cannot determine targetNote!");
            return null;
        }

        /* First, check if there's a note with the given key and return it if yes */
        for (int i = 0; i < music.notes.Count; i++)
        {
            if (music.notes[i].status == Note.Status.ACTIVATABLE || music.notes[i].status == Note.Status.RUINABLE)
            {
                if (music.notes[i].positionInSpace == LevelInformation.playerPosition || music.notes[i].positionInSpace == Note.Position.MIDDLE)
                {
                    if (music.notes[i].key == inputType || music.notes[i].key == InputHandler.Input.ANY)
                        return music.notes[i];
                }
            }
        }

        /* If not, then return the first note in threshold, except if it is a DASH (dash can't ruin other notes) */
        if (inputType == InputHandler.Input.DASH_LEFT || inputType == InputHandler.Input.DASH_RIGHT)
        {
            return null;
        }
        for (int i = 0; i < music.notes.Count; i++)
        {
            if (music.notes[i].status == Note.Status.ACTIVATABLE || music.notes[i].status == Note.Status.RUINABLE)
            {
                if (music.notes[i].positionInSpace == LevelInformation.playerPosition || music.notes[i].positionInSpace == Note.Position.MIDDLE) 
                {
                    return music.notes[i];
                }
            }
        }
        return null;
    }

    public void onMiss(int noteID)
    {
        Debug.Log("Missed note " + noteID);

        LevelInformation.handleScore(Note.Score.NONE);
        player.loseHealth(1);

        TextPooler.Instance.SpawnFromPool("Miss!", new Vector3(1, 0.3f, 0.3f), Vector3.zero, transform.rotation);
    }

    public void addNoteToStatistic(Note note)
    {
        double accuracy = note.getAccuracy(songPosition);
        accuracyStatistic.Add(accuracy);
    }

    public double getAverageAccuracy()
    {
        if (accuracyStatistic == null) return 0;
        if (accuracyStatistic.Count == 0) return 0;

        double sum = 0;
        for (int i = 0; i < accuracyStatistic.Count; i++)
        {
            sum += accuracyStatistic[i];
        }
        return sum / accuracyStatistic.Count;
    }
}
