using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    Controls controls;
    public Conductor conductor;
    public Player player;
    int joystickValue = 0;
    int triggerValue = 0;
    const float joystickDeadzone = 0.5f;
    const float triggerDeadzone = 0.3f;

    public enum Input
    {
        RIGHT,
        LEFT,
        UP,
        DOWN,
        DASH_RIGHT,
        DASH_LEFT,
        BASS,
        ANY,
    }

    void Awake()
    {
        controls = new Controls();

        controls.Gameplay.HitUp.performed += ctx => onHit(Input.UP, ctx);
        controls.Gameplay.HitDown.performed += ctx => onHit(Input.DOWN, ctx);
        controls.Gameplay.HitRight.performed += ctx => onHit(Input.RIGHT, ctx);
        controls.Gameplay.HitLeft.performed += ctx => onHit(Input.LEFT, ctx);
        controls.Gameplay.Bass.performed += ctx =>
        {
            if (ctx.ReadValue<float>() > triggerDeadzone)
            {
                if (triggerValue == 1) return;
                triggerValue = 1;
            }
            else
            {
                triggerValue = 0;
                return;
            }
            onHit(Input.BASS, ctx);
        };
        controls.Gameplay.Bass.canceled += ctx =>
        {
            triggerValue = 0;
        };
        controls.Gameplay.DashRight.performed += ctx =>
        {
            if (ctx.ReadValue<float>() > joystickDeadzone)
            {
                if (joystickValue == 1) return;
                joystickValue = 1;
            }
            else
            {
                joystickValue = 0;
                return;
            }
            onHit(Input.DASH_RIGHT, ctx);
            player.onDashRight();
        };
        controls.Gameplay.DashRight.canceled += ctx =>
        {
            joystickValue = 0;
        };
        controls.Gameplay.DashLeft.performed += ctx =>
        {
            if (ctx.ReadValue<float>() > joystickDeadzone)
            {
                if (joystickValue == -1) return;
                joystickValue = -1;
            }
            else
            {
                joystickValue = 0;
                return;
            }
            onHit(Input.DASH_LEFT, ctx);
            player.onDashLeft();
        };
        controls.Gameplay.DashLeft.canceled += ctx =>
        {
            joystickValue = 0;
        };
    }

    public void onHit(Input inputType, InputAction.CallbackContext context)
    {
        Note targetNote = conductor.findTargetNote(inputType);
        if (targetNote == null)
        {
            Debug.Log("No note to hit at the moment");
            return;
        }
        else
        {
            Note.Score hitScore = targetNote.Hit(inputType, conductor.songPosition, conductor.music.bpm);
            string text = "" + Math.Round(Math.Abs(conductor.songPositionInBeats - targetNote.positionInBeats), 2);

            LevelInformation.handleScore(hitScore);

            Vector3 color;
            switch (hitScore)
            {
                case Note.Score.CRITICAL:
                    color = new Vector3(1, 0.8f, 0);
                    text = "Critical" + (LevelInformation.criticalStrike > 1 ? " x" + LevelInformation.criticalStrike : "") + "!";
                    player.gainHealth(2);
                    break;
                case Note.Score.NORMAL:
                    color = new Vector3(0.3f, 0.9f, 0.3f);
                    text = "Hit!";
                    player.gainHealth(1);
                    break;
                case Note.Score.BARELY:
                    color = new Vector3(0.3f, 0.3f, 0.9f);
                    text = "Barely!";
                    break;
                case Note.Score.EARLY:
                    color = new Vector3(1, 0.7f, 0.7f);
                    text = "Early!";
                    player.loseHealth(1);
                    break;
                case Note.Score.LATE:
                    color = new Vector3(1, 0.7f, 0.7f);
                    text = "Late!";
                    player.loseHealth(1);
                    break;
                case Note.Score.WRONG_KEY:
                    color = new Vector3(1, 0.3f, 0.3f);
                    text = "Wrong!";
                    player.loseHealth(1);
                    break;
                case Note.Score.NONE:
                default:
                    color = new Vector3(0.3f, 0.3f, 0.3f);
                    text = "#?@&!";
                    break;
            }

            conductor.addNoteToStatistic(targetNote);

            TextPooler.Instance.SpawnFromPool(text, color, Vector3.zero, transform.rotation);
        }
    }

    private void OnEnable()
    {
        controls.Gameplay.Enable();
    }

    private void OnDisable()
    {
        controls.Gameplay.Disable();
    }
}
