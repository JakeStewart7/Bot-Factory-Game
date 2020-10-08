// Called by Health script when a unit runs out of health
// Takes care of unit destruction
// i.e. destroy healthbar, turn into scrap, destroy scrap connections

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitDestructor : MonoBehaviour
{
    public GameObject scrapPrefab;
    public int scrapOnDeath = 1;
    float scrapKnockback = 15f;

    public void DestroyUnit()
    {
        destroyScrapConnection();
        spawnScrap();
        playExplosionSound();
        
        if (gameObject.tag == "Player")
        {
            gameObject.GetComponent<Player>().RespawnPlayer();
            return;
        }

        destroyHealthBar();
        destroyBuildingUI();
        Destroy(gameObject);
    }

    void destroyScrapConnection()
    {
        // Stop scrap coming into building if building dies
        var t = gameObject.GetComponent<ScrapCollector>();
        if (t)
        {
            t.StopIncomingScrap();
        }

        // Destroy scrap connections if unit dies
        var s = gameObject.GetComponent<GrabScrap>();
        if (s)
        {
            s.DestroyScrapConnection();
        }
        else
        {
            return;
        }

        // Clear beingFetchedByEnemy bool if enemy dies so other enemies can target that scrap
        if (s.scrapTarget != null)
        {
            s.scrapTarget.GetComponent<BeingFetched>().ResetBeingFetchedByEnemy();
        }
    }

    void destroyHealthBar()
    {
        var h = gameObject.GetComponent<Health>();
        if (h)
        {
            gameObject.GetComponent<Health>().DestroyHealthBar();
        }
    }

    void spawnScrap()
    {
        if (gameObject.tag == "Player")
        {
            spawnPlayerScrap();
        }
        else if (gameObject.tag == "Ally Building" || gameObject.tag == "Enemy Building")
        {
            spawnBuildingScrap();
        }
        else
        {
            for (int i = 0; i < scrapOnDeath; i++)
            {
                Instantiate(scrapPrefab, transform.position, Quaternion.identity);
            }
        }
    }

    void spawnPlayerScrap()
    {
        Vector3 startingPosition = transform.position - new Vector3(0, (scrapOnDeath / 2f) / 10f, 0);
        for (int i = 0; i < scrapOnDeath; i++)
        {
            GameObject scrap = Instantiate(scrapPrefab, startingPosition + new Vector3(1 / 4f, i / 10f, 0), Quaternion.identity);
            scrap.AddComponent<ShockwaveEffect>();
            scrap.GetComponent<ShockwaveEffect>().initShockwaveEffect(transform.position, scrapKnockback);
        }
    }

    void spawnBuildingScrap()
    {
        scrapOnDeath += GetComponent<ScrapCount>().GetScrapCount();
        float scrapPrefabSize = scrapPrefab.GetComponent<SpriteRenderer>().bounds.size.x;

        int rows = Mathf.CeilToInt(Mathf.Sqrt(scrapOnDeath));;
        int columns = rows;
        Vector3 startPosition = transform.position + new Vector3(-scrapPrefabSize * (columns - 1) / 2, 
                                                                scrapPrefabSize * (rows - 1) / 2, 0);
        int count = scrapOnDeath;
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns && count > 0; j++)
            {
                Vector3 offset = new Vector3(j * scrapPrefabSize, -(i * scrapPrefabSize) + 0.01f, 0);
                GameObject scrap = Instantiate(scrapPrefab, startPosition + offset, Quaternion.identity);
                scrap.AddComponent<ShockwaveEffect>();
                scrap.GetComponent<ShockwaveEffect>().initShockwaveEffect(transform.position, scrapKnockback / 3);
                count--;
            }
        }
    }

    void destroyBuildingUI()
    {
        var s = GetComponent<ScrapCount>();  // Clear scrap count UI
        if (s) { s.SetScrapCount(0); }

        var t = GetComponent<ScrapTransfer>(); // Clear scrap transfer connection and scrap image being transferred
        if (t) { t.CancelTransfer(); }

        var u = GetComponent<UnitProduction>(); // Clear production icon on buildings making units
        if (u) { u.DeleteIcon(); }
    }

    void playExplosionSound()
    {
        if (gameObject.tag == "Player")
        {
            GameObject.Find("SoundPlayer").GetComponent<SoundManager>().PlaySound("PlayerExplosion");
        }
        else if (gameObject.tag == "Ally Building" || gameObject.tag == "Enemy Building")
        {
            GameObject.Find("SoundPlayer").GetComponent<SoundManager>().PlaySound("BuildingExplosion");
        }
        else if (gameObject.tag == "Neutral Attackable")  // Only play sound if players are in range
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject player in players)
            {
                if (Vector3.Distance(transform.position, player.transform.position) < 20f)
                {
                    GameObject.Find("SoundPlayer").GetComponent<SoundManager>().PlaySound("RockDestruction");
                    break;
                }
            }
        }
        else 
        {
            GameObject.Find("SoundPlayer").GetComponent<SoundManager>().PlaySound("UnitExplosion");
        }
    }
}
