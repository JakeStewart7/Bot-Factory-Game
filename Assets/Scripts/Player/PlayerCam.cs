using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCam : MonoBehaviour
{
    GameObject player;

    public void InitPlayerCam()
    {
        if (this.gameObject.name == "CameraPlayer1")
        {
            player = GameObject.Find("Player1");
        }
        else if (this.gameObject.name == "CameraPlayer2")
        {
            player = GameObject.Find("Player2");
        }
        if (player) { this.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, this.transform.position.z); }
    }
    // Start is called before the first frame update
    void Start()
    {
        InitPlayerCam();
    }

    // Update is called once per frame
    void Update()
    {
        if (!player)
        {
            InitPlayerCam();
        }
        else if (player.GetComponent<Player>().IsRespawning())
        {
            return;
        }
        else
        {
            float speed = Vector3.Distance(transform.position, player.transform.position) * 4;  // For lerp effect
            Vector3 position = Vector3.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);

            transform.position = new Vector3 (position.x, position.y, this.transform.position.z);  // Keep camera at -10 z-axis
        }
    }
}
