using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EndConditions : MonoBehaviour
{
    bool win;
    bool restarting = false;
    float restartTimer = 0f;
    public float restartTime = 5f;
    public Text endText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (restarting)
        {
            if (restartTimer < Time.time)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
            else { return; }
        }
        
        if (!hasBuildingWithTag("Enemy Building"))  // No enemy buildings remain, win
        {
            win = true;
            endActions();
        }
        else if (!hasBuildingWithTag("Ally Building")) // No ally buildings remain, lose
        {
            win = false;
            endActions();
        }
    }

    bool hasBuildingWithTag(string buildingTag)
    {
        GameObject[] buildings = GameObject.FindGameObjectsWithTag(buildingTag);
        if (buildings.Length < 1)
        {
            return false;
        }
        return true;
    }

    void endActions()
    {
        if (win == true)
        {
            endText.text = "VICTORY";
            endText.color = new Color(0, 214/255f, 255/255f);
        }
        else if (win == false)
        {
            endText.text = "DEFEAT";
            endText.color = new Color(255/255f, 29/255f, 25/255f);
        }
        restarting = true;
        restartTimer = restartTime + Time.time;
    }
}
