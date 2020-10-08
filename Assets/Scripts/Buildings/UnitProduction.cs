using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitProduction : MonoBehaviour
{
    public int UnitCost = 1;
    public float productionTime = 10f;
    float productionTimer = 0f;

    bool producing = false;

    public GameObject unitPrefab;
    public Canvas productionIconPrefab;
    Canvas productionIcon;
    Vector2 buildingSize;
    // Start is called before the first frame update
    void Start()
    {
        buildingSize = GetComponent<SpriteRenderer>().bounds.size;
    }

    // Update is called once per frame
    void Update()
    {
        if (GetComponent<ScrapCount>().GetScrapCount() - UnitCost >= 0 && producing == false)
        {
            startProduction();
        }
        else if (productionTimer < Time.time && producing == true)
        {
            endProduction();
        }

        if (productionIcon)  // If icon exists, update visual progress
        {
            updateIcon();
        }
    }

    void startProduction()
    {
        GetComponent<ScrapCount>().DecreaseScrapCount(UnitCost);
        createIcon();
        producing = true;

        productionTimer = productionTime + Time.time;
    }

    void endProduction()
    {
        spawnUnit();
        producing = false;
        if (GetComponent<ScrapCount>().GetScrapCount() > 0) 
        {
            Destroy(productionIcon.gameObject);
            startProduction();
        }
        else
        {
            Destroy(productionIcon.gameObject);
        }
    }

    void spawnUnit()
    {
        float unitSpawnOffset = (buildingSize.x / 2) + 1f;
        if (gameObject.tag == "Enemy Building") { unitSpawnOffset = -unitSpawnOffset; } // Spawn on left side
        GameObject unit = Instantiate(unitPrefab, transform.position + new Vector3 (unitSpawnOffset, 0, 0), Quaternion.identity);
        unit.name = unitPrefab.name;
    }

    void createIcon()
    {
        productionIcon = Instantiate(productionIconPrefab, transform.position, Quaternion.identity, GameObject.Find("IconHolder").transform);
        productionIcon.transform.localScale = new Vector3(buildingSize.x * 0.05f, buildingSize.y * 0.05f, 1);
        Vector3 iconOffset = new Vector3(buildingSize.x / 2.3f, buildingSize.y / 2.3f, 0);
        productionIcon.transform.position += iconOffset;

        if (gameObject.tag == "Enemy Building")
        {
            productionIcon.transform.GetChild(1).GetComponent<Image>().color = new Color(255, 0, 0, 156f/255);
        }
    }

    void updateIcon()
    {
        productionIcon.transform.GetChild(1).GetComponent<Image>().fillAmount = 1 - (productionTimer - Time.time) / productionTime;
    }

    public void DeleteIcon()
    {
        if (productionIcon)
        {
            Destroy(productionIcon.gameObject);
        }
    }

    public bool IsProducing()
    {
        return producing;
    }
}
