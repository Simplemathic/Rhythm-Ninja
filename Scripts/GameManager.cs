using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/*
 * Starts the game and the audio file based on the LevelInformation.songID variable
 */
public class GameManager : MonoBehaviour
{
    public Conductor conductor;
    public List<AudioClip> clips;
    public AudioClip testClip;

    private MusicData musicData;
    private Music music;

    void Awake()
    {
        if (LevelInformation.songID == null) return;

        int levelID = LevelInformation.songID ?? 0;

        switch (levelID)
        {
            case 0:
                music = new Music("Samurai", "Rameses B", 87, 1.483, 2.0, 1.0);
                break;
            case 1:
                music = new Music("Riddle", "Bad Computer", 128, 4.701, 0.5, 1.0);
                break;
            case 2:
                music = new Music("Cheat Codes", "Nitro Fun", 128, 0.085, 1.0, 2.0);
                break;
            case 3:
                music = new Music("Rattlesnake", "RoGue, Pegboard Nerds Remix", 175, 0.7, 1.0, 10.0);
                break;
            case -1:
                music = new Music("Offset Test", "Simplemathic", 124, 0.613, 2, 1);
                break;
            default:
                Debug.LogWarning("No level with ID \"" + levelID + "\" exist!");
                return;
        }

        if (levelID >= 0)
        {
            music.musicSource = clips[levelID];
        }
        else
        {
            music.musicSource = testClip;
        }
        
        LevelInformation.update(music.title, music.artist);
        LevelInformation.playerPosition = Note.Position.RIGHT;
        LevelInformation.survived = true;
        musicData = new MusicData();

        List<Note> notes = new List<Note>();

        for (double i = 0; i < 1000; i++)
            notes.Add(new Note(i / 4, InputHandler.Input.ANY, Note.Position.RIGHT));

        if (levelID < 3) 
            music.setNotes(musicData.getMusicData(levelID), conductor);
        else
            music.setNotes(notes, conductor);

        conductor.Play(music);
    }

    public void onDeath()
    {
        LevelInformation.survived = false;
        onMusicEnd();
    }

    public void onMusicEnd()
    {
        LevelInformation.averageAccuracy = conductor.getAverageAccuracy();
        SceneManager.LoadSceneAsync("Menu");
    }
}
