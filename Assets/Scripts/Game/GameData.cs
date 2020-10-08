using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoBehaviour
{
    int allyScrap;
    int enemyScrap;

    const int startingAllyScrap = 0;
    const int startingEnemyScrap = 0;

    Vector3 midpoint;
    // Start is called before the first frame update
    void Start()
    {
        allyScrap = startingAllyScrap;
        enemyScrap = startingEnemyScrap;
        midpoint = new Vector3(0, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        calculateEnemyMidpoint();
    }

    public void IncreaseAllyScrap(int amount)
    {
        allyScrap += amount;
    }

    public void DecreaseAllyScrap(int amount)
    {
        allyScrap -= amount;
    }

    public void IncreaseEnemyScrap(int amount)
    {
        enemyScrap += amount;
    }

    public void DecreaseEnemyScrap(int amount)
    {
        enemyScrap -= amount;
    }

    private void calculateEnemyMidpoint()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        midpoint = new Vector3(0, 0, 0);
        foreach (GameObject enemy in enemies)
        {
            midpoint += enemy.transform.position;
        }
        midpoint /= enemies.Length;
    }
    public Vector3 GetEnemyMidpoint()
    {
        return midpoint;
    }
}
