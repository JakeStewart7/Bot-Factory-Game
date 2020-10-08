using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrapCount : MonoBehaviour
{
    int scrapCount;
    public int StartingScrap = 0;
    public int rowCount;
    public GameObject scrapImagePrefab;
    float scrapSize;
    List<GameObject> scrapList;

    int columnCount;

    void initScrapList()
    {
        scrapCount = StartingScrap;
        scrapList = new List<GameObject>();
        scrapSize = scrapImagePrefab.GetComponent<SpriteRenderer>().bounds.size.x;
        columnCount = (int)Mathf.Floor(this.GetComponent<SpriteRenderer>().bounds.size.x / scrapImagePrefab.GetComponent<SpriteRenderer>().bounds.size.x);
        displayScrapCount();
    }

    // Start is called before the first frame update
    void Start()
    {
        initScrapList();
        displayScrapCount();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void displayScrapCount()
    {
        clearScrapDisplay();
        int count = scrapCount;  // For counting number of scrap left to draw
        // if (count > rowCount * columnCount) { count = rowCount * columnCount; }  // Don't draw more than can fit in building
        Vector3 startPosition = transform.position + new Vector3(-scrapSize * (columnCount - 1) / 2, 
                                                                 scrapSize * (rowCount - 1) / 2, 0);
        for (int i = 0; i < rowCount; i++)
        {
            for (int j = 0; j < columnCount && count > 0; j++)
            {
                Vector3 offset = new Vector3(j * scrapSize, -(i * scrapSize), 0);
                scrapList.Add(Instantiate(scrapImagePrefab, startPosition + offset, Quaternion.identity, GameObject.Find("ScrapImageHolder").transform));
                count--;
            }
        }
    }

    void clearScrapDisplay()
    {
        foreach (GameObject scrap in scrapList)
        {
            Destroy(scrap);
        }
        scrapList = new List<GameObject>();
        /*
        for (int i = 0; i < scrapList.Count; i++)
        {
            GameObject temp = scrapList[i];
            scrapList.Remove(temp);
            Destroy(temp);
        }
        */
    }

    public int GetScrapCount()
    {
        return scrapCount;
    }

    public bool HasSpace()
    {
        return scrapCount < (columnCount * rowCount);
    }

    public void SetScrapCount(int newCount)
    {
        scrapCount = newCount;
        if (scrapCount < 0) { scrapCount = 0; }
        displayScrapCount();
    }

    public void IncreaseScrapCount(int addedCount)
    {
        scrapCount += addedCount;
        if (scrapCount < 0) { scrapCount = 0; }
        displayScrapCount();
    }

    public void DecreaseScrapCount(int subtractedCount)
    {
        scrapCount -= subtractedCount;
        if (scrapCount < 0) { scrapCount = 0; }
        displayScrapCount();
    }
}
