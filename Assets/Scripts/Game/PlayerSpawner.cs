using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    public GameObject PlayerPrefab;
    public Sprite GreenPlayerSprite;
    public GameObject GreenScrapConnectionPrefab;
    public Sprite GreenCooldownShieldSprite;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnPlayer(Vector3 position, int playerNumber)
    {
        GameObject player = Instantiate(PlayerPrefab, position, Quaternion.identity, GameObject.Find("Players").transform);
        if (playerNumber == 1)
        {
            player.GetComponent<Player>().InitPlayer(playerNumber);  // Initialize name, controller num, etc.
        }
        else if (playerNumber == 2)
        {
            player.GetComponent<Player>().InitPlayer(playerNumber, GreenPlayerSprite, GreenScrapConnectionPrefab, GreenCooldownShieldSprite);
        }
    }
}
