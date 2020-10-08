using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllyRetriever : MonoBehaviour
{
    public enum AllyState {Roaming, Retrieving, Returning, Patrolling, FollowingPlayer};
    public AllyState allyState;  // PUBLIC FOR TESTING TO SEE IN INSPECTOR


    float speed;
    float baseSpeed = 1.9f;
    float maxSpeed = 4f;
    float nearbyDistance = 100f;  // Distance that defines scrap as nearby

    float speedRoamingFactor = 1.5f;
    float speedRetrievingFactor = 2f;
    float speedReturningFactor = 1.3f;
    float speedPatrollingFactor = 1f;
    float speedFollowingPlayerFactor = 1f;

    GameObject roamingTarget;
    GameObject scrapTarget;

    List<string> directionsRemaining;
    string currentDirection;
    float patrolTime = 8f;
    float patrolTimer = 0f;
    // Start is called before the first frame update
    void Start()
    {
        speed = baseSpeed * speedRoamingFactor;
        toStateRoaming();
    }

    // Update is called once per frame
    void Update()
    {
        switch(allyState)
        {
            case AllyState.Roaming:
                roamingAction();
                break;
            case AllyState.Retrieving:
                retrievingAction();
                break;
            case AllyState.Returning:
                returningAction();
                break;
            case AllyState.Patrolling:
                patrollingAction();
                break;
            case AllyState.FollowingPlayer:
                followingPlayerAction();
                break;
        }
    }

    /* =======================   Actions   ======================= */
    void roamingAction()
    {
        targetNearestScrap();  // Check for nearby scrap
        moveTowardsTarget();
    }

    void retrievingAction()
    {
        if (!targetExists())
        {
            toStateRoaming();
            return;
        }
        if (scrapTarget.GetComponent<ConnectionHandler>().HasConnection())
        {
            scrapTarget.GetComponent<BeingFetched>().ResetBeingFetchedByAlly();
            toStateRoaming();
        }
        if (scrapTarget && scrapTarget.tag == "Untagged")  // If a building has claimed it i.e. is pulling it in (see ScrapCollect.claimScrap)
        {
            toStateRoaming();
        }
        moveTowardsTarget();  // Move towards scrap
    }

    void returningAction()
    {
        moveTowardsTarget();  // Move towards base
        if (!HasConnection()) 
        {
            toStateRoaming();
        }
    }

    void patrollingAction()
    {
        targetNearestScrap();  // Check for nearby scrap
        if (patrolTimer < Time.time)
        {
            getNewDirection();
        }
        if      (currentDirection == "Right") { transform.position += new Vector3(1 * speed * Time.deltaTime, 0, 0); }
        else if (currentDirection == "Left") { transform.position += new Vector3(-1 * speed * Time.deltaTime, 0, 0); }
        else if (currentDirection == "Up") { transform.position += new Vector3(0, 1 * speed * Time.deltaTime, 0); }
        else if (currentDirection == "Down") { transform.position += new Vector3(0, -1 * speed * Time.deltaTime, 0); }
    }

    void followingPlayerAction()
    {
        moveTowardsPlayer();
        targetNearestScrap();  // Check for nearby scrap
    }
    /* =========================================================== */


    /* =======================   toState   ======================= */
    void toStateRoaming()
    {
        gameObject.GetComponent<GrabScrap>().scrapTarget = null;
        scrapTarget = GameObject.Find("Ally Base");
        speed = baseSpeed * speedRoamingFactor;
        allyState = AllyState.Roaming;
    }

    void toStateRetrieving(GameObject scrap)
    {
        scrapTarget = scrap;
        scrap.GetComponent<BeingFetched>().SetBeingFetchedByAlly();
        gameObject.GetComponent<GrabScrap>().scrapTarget = scrap;
        speed = baseSpeed * speedRetrievingFactor;
        allyState = AllyState.Retrieving;
    }

    void toStateReturning(GameObject building)
    {
        scrapTarget = building;
        speed = baseSpeed * speedReturningFactor;
        allyState = AllyState.Returning;
    }

    void toStatePatrolling()
    {
        // Patrolling specific
        directionsRemaining = new List<string>();
        currentDirection = "Right";
        patrolTimer = patrolTime + Time.time;

        // More generic
        speed = baseSpeed * speedPatrollingFactor;
        allyState = AllyState.Patrolling;
    }

    void toStateFollowingPlayer(GameObject target)
    {
        scrapTarget = target;
        speed = baseSpeed * speedFollowingPlayerFactor;
        allyState = AllyState.FollowingPlayer;
    }
    /* =========================================================== */

    void targetNearestScrap()
    {
        GameObject[] allScrap = GameObject.FindGameObjectsWithTag("Scrap");
        List<GameObject> nearbyScrapList = new List<GameObject>();
        foreach (GameObject scrap in allScrap)
        {
            if (Vector3.Distance(scrap.transform.position, this.transform.position) < nearbyDistance)
            {
                if (scrap.GetComponent<ConnectionHandler>().HasConnection() || scrap.GetComponent<BeingFetched>().BeingFetchedByAlly()) { continue; }
                nearbyScrapList.Add(scrap);
            }
        }
        
        if (nearbyScrapList.Count < 1) { return; }  // If no nearby scrap, return

        GameObject nearestScrap = nearbyScrapList[0];
        float nearestDistance = Vector3.Distance(nearbyScrapList[0].transform.position, this.transform.position);
        foreach (GameObject scrap in nearbyScrapList)
        {
            if (Vector3.Distance(scrap.transform.position, this.transform.position) < nearestDistance)
            {
                nearestScrap = scrap;
                nearestDistance = Vector3.Distance(scrap.transform.position, this.transform.position);
            }
        }
        
        if (isClosestAvailableRetriever(nearestScrap))
        {
            toStateRetrieving(nearestScrap);
        }
    }

    void targetNearbyBuilding()
    {
        GameObject allyBase = GameObject.Find("Ally Base");
        if (allyBase) // Default return to main base
        {
            toStateReturning(allyBase);
            return;
        }

        GameObject[] buildings = GameObject.FindGameObjectsWithTag("Ally Building");
        if (buildings.Length < 1)
        {
            GameObject obj = new GameObject();
            scrapTarget = obj;
            return;
        }
        float nearestDistance = Vector3.Distance(this.transform.position, buildings[0].transform.position);  // Set nearest to first building
        GameObject nearestBuilding = buildings[0];

        foreach (GameObject building in buildings)  // Find nearest building
        {
            float buildingDistance = Vector3.Distance(this.transform.position, building.transform.position);
            if (buildingDistance < nearestDistance)
            {
                nearestDistance = buildingDistance;
                nearestBuilding = building;
            }
        }
        toStateReturning(nearestBuilding);
    }

    void moveTowardsTarget()
    {
        if (scrapTarget)
        {
            transform.position = Vector3.MoveTowards(this.transform.position, scrapTarget.transform.position, speed * Time.deltaTime);
        }
    }
    
    void moveTowardsPlayer()
    {
        float tempSpeed = speed * (Vector3.Distance(scrapTarget.transform.position, transform.position) - 1f);
        if (tempSpeed > maxSpeed) { tempSpeed = maxSpeed; }  // Cap out max follow speed
        if (scrapTarget)
        {
            transform.position = Vector3.MoveTowards(this.transform.position, scrapTarget.transform.position, tempSpeed * Time.deltaTime);
        }
    }

    bool targetExists()
    {
        if (scrapTarget)
        {
            return true;
        }
        else { return false; }
    }

    void getNewDirection()
    {
        if (directionsRemaining.Count < 1)
        {
            directionsRemaining = new List<string>() {"Right", "Left", "Up", "Down"};;
        }
        int rand = Random.Range(0, directionsRemaining.Count - 1);
        currentDirection = directionsRemaining[rand];
        directionsRemaining.RemoveAt(rand);

        patrolTimer = patrolTime + Time.time;
    }

    bool isClosestAvailableRetriever(GameObject target)
    {
        List<GameObject> validRetrievers = new List<GameObject>();
        GameObject[] units = GameObject.FindGameObjectsWithTag("Ally");
        GameObject closestRetriever = null;
        foreach (GameObject unit in units)
        {
            var retrieverScript = unit.GetComponent<AllyRetriever>();
            if (retrieverScript && retrieverScript.GetState() != AllyState.Retrieving && retrieverScript.GetState() != AllyState.Returning)
            {
                validRetrievers.Add(unit);
            }
        }
        if (validRetrievers.Count > 0)
        {
            closestRetriever = validRetrievers[0];
        }
        foreach (GameObject unit in validRetrievers)
        {
            // See if the valid retriever is closer than the closest retriever
            if (Vector3.Distance(unit.transform.position, target.transform.position) <= Vector3.Distance(closestRetriever.transform.position, target.transform.position))
            {
                closestRetriever = unit;
            }
        }
        if (closestRetriever == this.gameObject)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag == "Scrap")
        {
            if (collider.GetComponent<ConnectionHandler>().HasConnection()) { return; }
            else 
            {
                collider.GetComponent<BeingFetched>().ResetBeingFetchedByAlly();
                targetNearbyBuilding();
            }
        }
        if (collider.tag == "Signal")
        {
            if (allyState == AllyState.Roaming || allyState == AllyState.Patrolling)               // Advancing state goes to FollowingPlayer state
            {
                GetComponent<SpawnSignalIcon>().SpawnSignal();
                GameObject.Find("SoundPlayer").GetComponent<SoundManager>().PlaySound("SignalResponse");
                toStateFollowingPlayer(collider.GetComponent<PlayerSignal>().GetOwner());
            }
            else if (allyState == AllyState.FollowingPlayer)
            {
                GetComponent<SpawnSignalIcon>().SpawnSignal();
                GameObject.Find("SoundPlayer").GetComponent<SoundManager>().PlaySound("SignalResponse");
                toStateRoaming();
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collider)
    {
        if (collider.gameObject.tag == "Ally Building" && (allyState == AllyState.Returning || allyState == AllyState.Roaming))
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            GameObject closestPlayer = players[0];
            foreach(GameObject player in players)
            {
                if (Vector3.Distance(player.transform.position, transform.position) < 
                    Vector3.Distance(closestPlayer.transform.position, transform.position))
                {
                    closestPlayer = player;
                }
            }
            toStateFollowingPlayer(closestPlayer);
        }
    }

    public GameObject GetTarget()
    {
        return scrapTarget;
    }

    public AllyState GetState()
    {
        return allyState;
    }

    public void ResetState()
    {
        toStateRoaming();
    }

    public void Return()
    {
        targetNearbyBuilding();
    }

    public bool HasConnection()
    {
        GameObject s = gameObject.GetComponent<GrabScrap>().scrap;
        if (s)
        {
            return true;
        }
        return false;
    }
}
