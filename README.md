# Rhythm Ninja
A rhythm game developed in Unity during the Project Laboratory course.
 
A short video showcasing the gameplay can be found here: https://youtu.be/b8mS1fChd5o
 
This repository contains only the script files of the Unity project. 
I was unable to upload the whole project due to its large size, but all the script files can be found here.

Furthermore, I apologize for the potential ads appearing at the beginning of the above linked showcase video, the music I used for the game is copyrighted on YouTube.
 
## Content

### Conductor
The class that "conducts" the flow of music. Contains variables describing the current position inside the audio file, moves the Note objects, calculates the accuracy of the player imput.

### GameManager
Starts the audio file corresponding to the selected level

### InputHandler
Detects input events and calls the adequate functions

### LevelInformation
Contains global variables such as constants and statistics as well as level progression information

### MenuManager
Handles the navigation between menus (activates and disables menu GameObjects)

### MusicData
Represents levels as lists of Notes

### Music
Represents an audio file:
 - information (title, bpm, author...)
 - basic behaviours 

### Note
Represents a note the player has to "hit":
 - its position in time
 - type (which key to press)
 - position (where it is on the screen)
 - state (already hit, not yet hit, missed etc.)
 - basic behaviours

### ObjectMover
This script moves the notes downwards

### ObjectPooler
Something like a threadpool for the note GameObjects (not to create an individual gameObject to every single Note)

### Player
Represents the Player and their state and behaviour

### Settings
Stores the settings (volume and input delay) as global variables, implements saving for these data (using json serialization)

### TextBoing
Just a fun animation for the title screens yellow text, inspired from Minecraft

### TextMover
Moves the hit response texts (in the middle of the screen) upwards

### TextPooler
Just like ObjectPooler for Texts
