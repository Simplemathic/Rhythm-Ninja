using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class LevelSummaryMenu : MonoBehaviour
{
    public Text tTitle;
    public Text tArtist;
    public Text tScore;
    public Text tCriticalCount;
    public Text tHitCount;
    public Text tBarelyCount;
    public Text tMissCount;
    public RectTransform background;

    private Color oldBgColor;

    // Display some interesting data collected during gameplay
    public void updateSummary()
    {
        oldBgColor = background.GetComponent<Image>().color;
        
        if (!LevelInformation.survived)
        {
            Debug.Log("Death");
            background.GetComponent<Image>().color = new Color(0.9f, 0.3f, 0.2f);
        }

        tTitle.text = LevelInformation.levelTitle;
        tArtist.text = LevelInformation.artist;
        tScore.text = LevelInformation.score.ToString();

        int totalCount = LevelInformation.criticalCount + LevelInformation.hitCount + LevelInformation.barelyCount + LevelInformation.missCount;

        tCriticalCount.text = LevelInformation.criticalCount + " (" + (totalCount > 0 ? Math.Round((double)LevelInformation.criticalCount / totalCount * 100, 0) : 0) + "%)";
        tHitCount.text      = LevelInformation.hitCount      + " (" + (totalCount > 0 ? Math.Round((double)LevelInformation.hitCount      / totalCount * 100, 0) : 0) + "%)";
        tBarelyCount.text   = LevelInformation.barelyCount   + " (" + (totalCount > 0 ? Math.Round((double)LevelInformation.barelyCount   / totalCount * 100, 0) : 0) + "%)";
        tMissCount.text     = LevelInformation.missCount     + " (" + (totalCount > 0 ? Math.Round((double)LevelInformation.missCount     / totalCount * 100, 0) : 0) + "%)";
    }

    public void onBackButton()
    {
        LevelInformation.songID = null;
        
        background.GetComponent<Image>().color = oldBgColor;
    }
}
