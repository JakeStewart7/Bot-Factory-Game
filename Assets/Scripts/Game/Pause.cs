using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pause : MonoBehaviour
{
    bool isPaused = false;
    public Text pauseText;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Pause"))
        {
            isPaused = !isPaused;

            if (isPaused)
            {
                Time.timeScale = 0;
                pauseText.enabled = true;
            }
            else if (!isPaused)
            {
                pauseText.enabled = false;
                Time.timeScale = 1;
            }
        }
    }
}
