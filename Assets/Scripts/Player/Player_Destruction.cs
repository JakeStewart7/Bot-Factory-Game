using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Player : MonoBehaviour
{
    bool respawning = false;
    public float respawnTime = 4f;
    float respawnTimer = 0f;

    float allyBaseSize;
    Vector3 allyBasePosition;

    void destructionStart()
    {
        GameObject allyBase = GameObject.Find("Ally Base");
        allyBaseSize = allyBase.GetComponent<SpriteRenderer>().bounds.size.y;
        allyBasePosition = allyBase.transform.position;
    }

    // Update is called once per frame
    void destructionUpdate()
    {
        if (respawning == true && respawnTimer < Time.time)
        {
            respawning = false;
            returnPlayer();
        }
    }

    // Precursor: UnitDestructor is called and removes healthbar, scrap connections, and spawns scrap
    // This function respawns the player
    public void RespawnPlayer()
    {
        respawnTimer = Time.time + respawnTime;
        respawning = true;
        removePlayer();
        healPlayer();
        detachUnits();
    }

    void removePlayer()
    {
        transform.position = new Vector3(-100f, 0f, 0f);
    }

    void returnPlayer()
    {
        Vector3 respawnOffset = new Vector3(0, allyBaseSize / 2 + 2, 0);
        transform.position = allyBasePosition + respawnOffset;
    }

    void healPlayer()
    {
        GetComponent<Health>().FullHeal();
    }

    void detachUnits()
    {
        GameObject[] units = GameObject.FindGameObjectsWithTag("Ally");
        foreach (GameObject unit in units)
        {
            var s = unit.GetComponent<AllySmall>();
            if (s)
            {
                if (s.GetTarget() == this.gameObject)
                {
                    s.ResetState();
                }
            }
            var t = unit.GetComponent<AllyRetriever>();
            if (t)
            {
                if (t.GetTarget() == this.gameObject)
                {
                    t.ResetState();
                }
            }
        }
    }

    public bool IsRespawning()
    {
        return respawning;
    }
}
