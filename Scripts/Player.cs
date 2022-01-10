using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

/*
 * Contains the Player's behaviour and states
 */
public class Player : MonoBehaviour
{
    public const int maxHealth = 100;
    public int health;
    public RectTransform pHealth;
    public GameManager gm;

    private const int healthLossAmount = 10;
    private const int healthGainAmount = 1;

    void Start()
    {
        health = maxHealth;
    }

    void Update()
    {
        pHealth.offsetMax = new Vector2(health * 2 - 200, pHealth.offsetMax.y);
    }

    public void onDashRight()
    {
        transform.position = new Vector3(
            4,
            transform.position.y,
            transform.position.z
        );
        transform.localScale = new Vector3(
            Math.Abs(transform.localScale.x),
            transform.localScale.y,
            transform.localScale.z
        );
        LevelInformation.playerPosition = Note.Position.RIGHT;
    }

    public void onDashLeft()
    {
        transform.position = new Vector3(
            -4,
            transform.position.y,
            transform.position.z
        );
        transform.localScale = new Vector3(
            -1 * Math.Abs(transform.localScale.x),
            transform.localScale.y,
            transform.localScale.z
        );
        LevelInformation.playerPosition = Note.Position.LEFT;
    }

    public void loseHealth(int nTimes)
    {
        health -= healthLossAmount * nTimes;
        if (health < 0)
        {
            health = 0;
            gm.onDeath();
        }
    }

    public void gainHealth(int nTimes)
    {
        health += healthGainAmount * nTimes;
        if (health > maxHealth) health = maxHealth;
    }

    public void setHealthToMax()
    {
        health = maxHealth;
    }
}
