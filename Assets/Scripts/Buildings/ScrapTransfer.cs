using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrapTransfer : MonoBehaviour
{
    GameObject target;
    GameObject movingScrap;
    GameObject connectionLine;

    public GameObject scrapImagePrefab;
    public GameObject connectionLinePrefab;

    float intervalTime;
    float intervalTimer = 0f;
    float transferTime;
    float transferTimer = 0f;
    public float transferSpeed = 3f;

    bool isEnemy = false;

    // Start is called before the first frame update
    void Start()
    {
        if (this.gameObject.tag == "Enemy Base" || this.gameObject.tag == "Enemy Building")
        {
            isEnemy = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!target) { CancelTransfer(); }  // If target gets destroyed, cancel transfer

        // If this building has scrap to transfer and isn't on cooldown
        if (GetComponent<ScrapCount>().GetScrapCount() > 0 && intervalTimer < Time.time)
        {
            // Target lowest scrap building. If no building found, return
            if (!targetLowestBuilding()) 
            {
                return;
            }
            else  // If building is found, set transfer time based on target distance from this building
            {
                if (target) { transferTime = Vector3.Distance(transform.position, target.transform.position) / transferSpeed; } // Time it takes to get to target
                if (target) { intervalTime = transferTime + 0.7f; }
            }

            // If target selected has space for more scrap
            if (target && target.GetComponent<ScrapCount>().HasSpace())
            {
                transferScrap();
                GetComponent<ScrapCount>().DecreaseScrapCount(1);
                intervalTimer = Time.time + intervalTime;
            }
        }
        // If transfer is complete, destroy scrap and connection, and add 1 to other building scrap pile
        if (transferTimer < Time.time)
        {
            if (movingScrap)
            {
                Destroy(movingScrap);
                Destroy(connectionLine);
                target.GetComponent<ScrapCount>().IncreaseScrapCount(1);
            }
        }
    }

    void transferScrap()
    {
        // Send scrap to building
        movingScrap = Instantiate(scrapImagePrefab, transform.position, Quaternion.identity);
        movingScrap.AddComponent<MovingScrap>();
        movingScrap.GetComponent<MovingScrap>().initMovingScrap(target, transferSpeed);
        movingScrap.GetComponent<SpriteRenderer>().sortingLayerName = "Background";

        // Create connection between buildings
        connectionLine = Instantiate(connectionLinePrefab, (transform.position + target.transform.position) / 2, Quaternion.identity);
        connectionLine.transform.right = target.transform.position - transform.position;
        connectionLine.transform.localScale = new Vector3(Vector3.Distance(transform.position, target.transform.position),
                                                               connectionLine.transform.localScale.y / transform.localScale.y,
                                                               connectionLine.transform.localScale.z);
        transferTimer = Time.time + transferTime;
    }

    bool targetLowestBuilding()
    {
        List<GameObject> buildings = new List<GameObject>();  // Excludes ally retriever factory
        GameObject[] b;     // Get all buildings
        if (isEnemy == false)
        {
            b = GameObject.FindGameObjectsWithTag("Ally Building");
        }
        else
        {
            b = GameObject.FindGameObjectsWithTag("Enemy Building");
        }

        foreach (GameObject building in b)
        {
            /*
            if (building.name == "Ally Retriever Factory") { continue; }  // Skip over ally retriever factory
            else { buildings.Add(building); }
            */
            buildings.Add(building);  // Include Ally Retriever Factory in scrap transferring
        }

        if (buildings.Count <= 0) { return false; }  // If no buildings found, return false

        // Of all empty non producing buildings, choose one at random to give scrap to
        List<GameObject> dormantBuildings = findDormantBuildings(buildings);
        if (dormantBuildings.Count > 0)
        {
            int rand = Random.Range(0, dormantBuildings.Count);
            target = dormantBuildings[rand];
            return true;
        }
        
        // Find building with lowest amount of scrap to transfer to
        int lowestScrapCount = buildings[0].GetComponent<ScrapCount>().GetScrapCount();
        foreach (GameObject building in buildings)
        {
            // If building produces units but is not producing anything (empty), give scrap
            if (building.GetComponent<UnitProduction>() && !building.GetComponent<UnitProduction>().IsProducing())
            {
                target = building;
                break;
            }
            if (building.GetComponent<ScrapCount>().GetScrapCount() < lowestScrapCount)
            {
                lowestScrapCount = building.GetComponent<ScrapCount>().GetScrapCount();
                target = building;
            }
        }
        return true;
    }

    List<GameObject> findDormantBuildings(List<GameObject> buildings)
    {
        List<GameObject> dormantBuildings = new List<GameObject>();
        foreach (GameObject building in buildings)
        {
            if (building.GetComponent<UnitProduction>() && !building.GetComponent<UnitProduction>().IsProducing())
            {
                dormantBuildings.Add(building);
            }
        }
        return dormantBuildings;
    }

    public void CancelTransfer()
    {
        if (movingScrap)
        {
            Destroy(movingScrap);
        }
        if (connectionLine)
        {
            Destroy(connectionLine);
        }
    }
}
