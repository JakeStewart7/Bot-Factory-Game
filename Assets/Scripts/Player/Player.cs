using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Player : MonoBehaviour
{
    public int controllerNum;

    // Default is Player1 sprites (i.e. blue), so set to Player1 and don't change anything else unless given parameters
    public void InitPlayer(int playerNumber = 1, Sprite playerSprite = null, GameObject scrapConnectionPrefab = null, Sprite cooldownShieldSprite = null)
    {
        if (playerNumber == 1)
        {
            gameObject.name = "Player1";
        }
        else if (playerNumber == 2)
        {
            gameObject.name = "Player2";
        }
        controllerNum = playerNumber;
        if (playerSprite != null) { GetComponent<SpriteRenderer>().sprite = playerSprite; }
        if (scrapConnectionPrefab != null) { GetComponent<GrabScrap>().scrapConnectionLinePrefab = scrapConnectionPrefab; }
        if (cooldownShieldSprite != null) { GetComponent<CooldownBar>().CooldownBarSprite = cooldownShieldSprite; }
    }
    // Start is called before the first frame update
    void Start()
    {
        movementStart();
        laserPointerStart();
        attackStart();
        destructionStart();
    }

    // Update is called once per frame
    void Update()
    {
        movementUpdate();
        laserPointerUpdate();
        attackUpdate();
        destructionUpdate();
    }
}
