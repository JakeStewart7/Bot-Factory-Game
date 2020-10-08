using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrapCollector : MonoBehaviour
{
    string myTag;
    List<ScrapTimer> incomingScrap;

    struct ScrapTimer
    {
        public GameObject scrap;
        public float timer;
    };

    // Start is called before the first frame update
    void Start()
    {
        myTag = gameObject.tag;
        incomingScrap = new List<ScrapTimer>();
    }

    // Update is called once per frame
    void Update()
    {
        pullScrap();
    }

    void claimScrap(GameObject scrap)
    {
        ScrapTimer scrapTimer = new ScrapTimer();
        scrapTimer.scrap = scrap;
        scrapTimer.timer = Time.time + 2f;  // Time it takes to bring scrap in and collect it
        
        // Destroy Connection if exists
        GameObject owner = scrapTimer.scrap.GetComponent<ConnectionHandler>().GetOwner();
        if (owner != null)
        {
            owner.GetComponent<GrabScrap>().DestroyScrapConnection();
        }
        scrapTimer.scrap.tag = "Untagged";
        scrapTimer.scrap.GetComponent<BoxCollider2D>().enabled = false;
        incomingScrap.Add(scrapTimer);
    }

    void pullScrap()
    {
        if (incomingScrap != null && incomingScrap.Count > 0)
        {
            for (int i = 0; i < incomingScrap.Count; i++)
            {
                if (incomingScrap[i].timer > Time.time)
                {
                    incomingScrap[i].scrap.transform.position = 
                        Vector3.MoveTowards(incomingScrap[i].scrap.transform.position, transform.position, 5f * Time.deltaTime);
                }
                else
                {
                    collectScrap(incomingScrap[i]);
                }
            }
        }
    }

    void collectScrap(ScrapTimer scrapTimer)
    {
        // Stop any enemy chasing this scrap
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach(GameObject enemy in enemies)
        {
            if (enemy.GetComponent<EnemySmall>())
            {
                if (enemy.GetComponent<EnemySmall>().GetTarget() == scrapTimer.scrap)
                {
                    enemy.GetComponent<EnemySmall>().ResetState();
                }
            }
        }
        // Stop any Ally chasing this scrap
        GameObject[] allies = GameObject.FindGameObjectsWithTag("Ally");
        foreach(GameObject ally in allies)
        {
            if (ally.GetComponent<AllyRetriever>())
            {
                if (ally.GetComponent<AllyRetriever>().GetTarget() == scrapTimer.scrap)
                {
                    ally.GetComponent<AllyRetriever>().ResetState();
                }
            }
        }

        // Add scrap to building's scrap pile
        GetComponent<ScrapCount>().IncreaseScrapCount(1);
        
        Destroy(scrapTimer.scrap);         // Destroy scrap
        incomingScrap.Remove(scrapTimer);  // Remove scrap from list
    }

    bool containsScrap(GameObject scrap)
    {
        foreach (ScrapTimer scrapTimer in incomingScrap)
        {
            if (scrapTimer.scrap == scrap)
            {
                return true;
            }
        }
        return false;
    }

    public void StopIncomingScrap()
    {
        foreach (ScrapTimer scrapTimer in incomingScrap)
        {
            scrapTimer.scrap.tag = "Scrap";
            scrapTimer.scrap.GetComponent<BoxCollider2D>().enabled = true;
        }
    }

    void OnTriggerEnter2D(Collider2D collider)  // If scrap touches the building, claim the scrap
    {
        if (collider.tag == "Scrap" && !containsScrap(collider.gameObject))
        {
            claimScrap(collider.gameObject);
        }
    }

    void OnCollisionEnter2D(Collision2D collider)   // If player touches building, claim all scrap attached to player
    {
        GameObject scrap = null;
        var s = collider.gameObject.GetComponent<GrabScrap>();
        if (s)
        {
            scrap = collider.gameObject.GetComponent<GrabScrap>().scrap;
        }
        if (scrap != null && scrap.GetComponent<ConnectionHandler>().HasConnection())
        {
            claimScrap(scrap);
        }
    }
}
