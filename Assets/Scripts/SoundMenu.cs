using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundMenu : MonoBehaviour
{
    public static float ValSfx = 0.5f;
    public static float ValMusic = 0.5f;
    public static float ValMaster = 0.5f;
    void Start()
    {
    }
    void Update()
    {
    }
    public void setMasterVolume(float newVal)
    {
        ValMaster = newVal;
        Debug.Log("SFX: " + ValSfx + " Music: " + ValMusic + " Master: " + ValMaster);
    }
    public void setSfxVolume(float newVal)
    {
        ValSfx = newVal;
        Debug.Log("SFX: " + ValSfx + " Music: " + ValMusic + " Master: " + ValMaster);
    }
    public void setMusicVolume(float newVal)
    {
        ValMusic = newVal;
        Debug.Log("SFX: " + ValSfx + " Music: " + ValMusic + " Master: " + ValMaster);
    }
}
