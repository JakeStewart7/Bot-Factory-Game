// Gives an object the behavior to trigger a collision with scrap and create a connection line
// that will keep the scrap following the player until they return it to base. 
// One connection at a time.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabScrap : MonoBehaviour
{
    float dragSpeed = 1.5f;
    bool scrapConnection = false;

    [HideInInspector]
    public GameObject scrap;
    [HideInInspector]
    public GameObject scrapTarget; // ** For units that go after scrap
    GameObject scrapConnectionLine;
    public GameObject scrapConnectionLinePrefab;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (scrapConnection == true)
        {
            updateScrapPosition();
            updateScrapConnectionLinePosition();
        }
    }

    // Check scrap doesn't already have a connection
    // Call scrap ConnectionHandler Connect() function to claim connection
    void createScrapConnection(GameObject scrapReference)
    {
        if (scrapReference.GetComponent<ConnectionHandler>().HasConnection()) { return; }  // If scrap already has connection, deny request

        scrapConnection = true; // Connection is made, only 1 allowed
        scrap = scrapReference; // Keep reference to scrap
        scrap.GetComponent<ConnectionHandler>().Connect(gameObject);
        scrapConnectionLine = Instantiate(scrapConnectionLinePrefab, (transform.position + scrap.transform.position) / 2, Quaternion.identity);
    }

    public void DestroyScrapConnection()
    {
        if (scrapConnection == true)
        {
            scrapConnection = false;
            scrap.GetComponent<ConnectionHandler>().Disconnect();
            if (scrapConnectionLine)
            {
                Destroy(scrapConnectionLine);
            }
            scrap = null;
        }
    }

    void updateScrapPosition()
    {
        scrap.transform.position = Vector3.MoveTowards(scrap.transform.position, transform.position, 
                                        dragSpeed * Time.deltaTime * (Vector3.Distance(scrap.transform.position, transform.position) - 1.2f)); 
    }

    void updateScrapConnectionLinePosition()
    {
        scrapConnectionLine.transform.position = (transform.position + scrap.transform.position) / 2;   // midpoint
        scrapConnectionLine.transform.right = scrap.transform.position - transform.position;
        scrapConnectionLine.transform.localScale = new Vector3(Vector3.Distance(transform.position, scrap.transform.position),
                                                               scrapConnectionLine.transform.localScale.y,
                                                               scrapConnectionLine.transform.localScale.z);
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag == "Scrap" && scrapConnection == false)
        {
            createScrapConnection(collider.gameObject);
        }
    }

    public bool HasScrapConnection()
    {
        return scrapConnection;
    }
}
