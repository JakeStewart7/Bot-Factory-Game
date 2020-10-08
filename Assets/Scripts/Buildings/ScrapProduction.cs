using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrapProduction : MonoBehaviour
{
    public float spawnRate;
    float releasePower;

    Color darknessFactor;
    float lightIncreaseRate;

    public bool BlockedByRock = true;
    public GameObject scrapPrefab;
    public GameObject rockPrefab;
    GameObject scrap;
    GameObject rock;
    int scrapSortingOrder;
    
    // Start is called before the first frame update
    void Start()
    {
        releasePower = 7f;
        darknessFactor = new Color(0, 0, 0);
        lightIncreaseRate = 1 / spawnRate;

        if (BlockedByRock == true)
        {
            rock = Instantiate(rockPrefab, transform.position, Quaternion.identity);
            rock.name = "Rock";
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (rock) { return; }  // Don't spawn scrap until rock is destroyed

        if (scrap)  // If currently producing scrap
        {
            scrap.GetComponent<SpriteRenderer>().color = darknessFactor;
            darknessFactor += new Color(lightIncreaseRate * Time.deltaTime, lightIncreaseRate * Time.deltaTime, lightIncreaseRate * Time.deltaTime);
        }
        else
        {
            beginScrapProduction();
        }

        if (darknessFactor.grayscale > 1f)  // If done producing, release
        {
            releaseScrap();
        }
    }

    void beginScrapProduction()
    {
        scrap = Instantiate(scrapPrefab, transform.position, Quaternion.identity);
        scrapSortingOrder = scrap.GetComponent<SpriteRenderer>().sortingOrder;

        scrap.GetComponent<BoxCollider2D>().enabled = false;
        scrap.tag = "Untagged";
        scrap.GetComponent<SpriteRenderer>().sortingOrder = this.GetComponent<SpriteRenderer>().sortingOrder + 1;
        scrap.GetComponent<SpriteRenderer>().color = darknessFactor;
    }

    void releaseScrap()
    {
        scrap.GetComponent<BoxCollider2D>().enabled = true;
        scrap.tag = "Scrap";
        scrap.GetComponent<SpriteRenderer>().sortingOrder = scrapSortingOrder;
        shockwaveScrap();

        scrap = null;
        darknessFactor = new Color(0, 0, 0);
    }

    void shockwaveScrap()
    {
        Vector3 randDir = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);

        var s = scrap.AddComponent<ShockwaveEffect>();
        s.initShockwaveEffect(transform.position + randDir, releasePower);
    }
}
