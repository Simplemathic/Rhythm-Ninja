using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;

public class OptionsMenu : MonoBehaviour
{
    private string settingsPath = "";

    public Slider offsetSlider;
    public Text offsetValue;

    public Slider volumeSlider;
    public Text volumeValue;

    void Awake()
    {
        updateSettingPath();
    }

    public void updateSettingPath()
    {
        settingsPath = Application.dataPath + "/setting.txt";
    }

    public void onOffsetSliderValueChanged()
    {   
        Settings.inputOffset = offsetSlider.value / 1000;
        offsetValue.text = offsetSlider.value + " ms";
    }

    public void onVolumeSliderValueChanged()
    {
        Settings.volume = volumeSlider.value / 100;
        volumeValue.text = volumeSlider.value + "";
    }

    public void updateOffsetSliderValue()
    {
        offsetValue.text = Settings.inputOffset * 1000 + " ms";
        offsetSlider.value = (float)Settings.inputOffset * 1000;
    }

    public void updateVolumeSliderValue()
    {
        volumeValue.text = Settings.volume * 100 + "";
        volumeSlider.value = Settings.volume * 100;
    }

    public void onTestButtonPressed()
    {
        Settings.inputOffset = 0;
    }

    public void Save()
    {
        SaveObject saveObject = new SaveObject
        {
            volume = (float)Math.Round(Settings.volume, 2) * 100,
            inputOffset = Settings.inputOffset * 1000
        };

        string json = JsonUtility.ToJson(saveObject);

        File.WriteAllText(settingsPath, json);
    }

    public void Load()
    {
        if (File.Exists(settingsPath))
        {
            SaveObject saveObject = JsonUtility.FromJson<SaveObject>(File.ReadAllText(settingsPath));
            
            Settings.inputOffset = saveObject.inputOffset / 1000;
            Settings.volume = saveObject.volume / 100;
        }
        else
        {
            Settings.inputOffset = Settings.defInputOffset;
            Settings.volume = Settings.defVolume;
        }
        updateOffsetSliderValue();
        updateVolumeSliderValue();
    }

    private class SaveObject
    {
        public float volume;
        public double inputOffset;
    }
}
