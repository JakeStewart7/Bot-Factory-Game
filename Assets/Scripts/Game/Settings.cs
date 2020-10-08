using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings : MonoBehaviour
{
    public bool Testing = false;
    public bool PlayMusic = true;
    public bool PlaySound = true;

    public Camera CameraTest;
    public Camera CameraPlayer1;
    public Camera CameraPlayer2;
    // Start is called before the first frame update
    void Start()
    {
        setScripts();
        setCameras();
        setMusic();
        setSound();
    }

    void setScripts()
    {
        if (Testing == true)
        {
            GetComponent<MapCreation>().enabled = false;
            GetComponent<EndConditions>().enabled = false;
        }
        else if (Testing == false)
        {
            GetComponent<MapCreation>().enabled = true;
            GetComponent<EndConditions>().enabled = true;
        }
    }

    void setCameras()
    {
        if (Testing == true)
        {
            CameraTest.GetComponent<Camera>().enabled = true;
            CameraPlayer1.GetComponent<Camera>().enabled = false;
            CameraPlayer2.GetComponent<Camera>().enabled = false;
        }
        else if (Testing == false)
        {
            CameraTest.GetComponent<Camera>().enabled = false;
            CameraPlayer1.GetComponent<Camera>().enabled = true;
            CameraPlayer2.GetComponent<Camera>().enabled = true;
        }
    }

    void setMusic()
    {
        if (PlayMusic == true)
        {
            GameObject.Find("MusicPlayer").GetComponent<AudioSource>().enabled = true;
        }
        else if (PlayMusic == false)
        {
            GameObject.Find("MusicPlayer").GetComponent<AudioSource>().enabled = false;
        }
    }

    void setSound()
    {
        if (PlaySound == true)
        {
            GameObject.Find("SoundPlayer").GetComponent<AudioSource>().enabled = true;
        }
        else if (PlaySound == false)
        {
            GameObject.Find("SoundPlayer").GetComponent<AudioSource>().enabled = false;
        }
    }
}
