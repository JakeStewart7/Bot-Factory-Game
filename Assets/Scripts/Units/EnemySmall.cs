using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySmall : MonoBehaviour
{
    public enum EnemyState { Advancing, ChasingPlayer, AttackingUnit, AttackingBuilding, MoveToTarget, GettingScrap, ReturningScrap };
    public EnemyState enemyState;  // PUBLIC FOR TESTING TO SEE IN INSPECTOR

    public float baseSpeed = 2f;
    float speed;

    float speedAttackingFactor = 0.5f;
    float speedChasingFactor = 1.9f;
    float speedRetrievingFactor = 2.1f;
    float speedAttackingBuildingFactor = 1.3f;
    float buildingAttackSpeedFactor = 1.3f;  // Increased attack speed on buildings
    float clumpFactor = 0.16f;  // How much enemies drift towards each other

    float attackTimer = 0f;
    float strafeTimer = 0f;
    public float attackTime;
    float buildingAttackTime; // Increased attack speed on buildings
    float beforeAttackTime = 0.5f;
    float afterAttackTime = 0.5f;
    float strafeTime = 1.5f;

    bool firedRecently = false;
    int strafeDirection = 1;  // Positive direction or negative

    public GameObject shortLaserPrefab;

    float nearbyPlayerDistance = 10f;  // Distance that defines a player as nearby
    float nearbyScrapDistance = 10f;
    float nearbyBuildingDistance = 10f;
    float sightDistance = 11f;  // Distance where enemy stops chasing player
    float attackRange = 7f;  // For chasing players, range unit will start attacking

    GameObject target;
    Vector3 midpointDir;

    // Start is called before the first frame update
    void Start()
    {
        beforeAttackTime = attackTime * (1/4f);
        afterAttackTime = attackTime * (3/4f);
        buildingAttackTime = attackTime / buildingAttackSpeedFactor;

        toStateAdvancing();
    }

    // Update is called once per frame
    void Update()
    {
        switch(enemyState)
        {
            case EnemyState.Advancing:
                advancingAction();
                break;
            case EnemyState.ChasingPlayer:
                chasingPlayerAction();
                break;
            case EnemyState.AttackingUnit:
                attackingUnitAction();
                break;
            case EnemyState.AttackingBuilding:
                attackingBuildingAction();
                break;
            case EnemyState.MoveToTarget:
                moveToTargetAction();
                break;
            case EnemyState.GettingScrap:
                gettingScrapAction();
                break;
            case EnemyState.ReturningScrap:
                returningScrapAction();
                break;
        }
    }

    /* ========================= STATE ACTIONS =========================*/
    void advancingAction()
    {
        midpointDir = (GameObject.FindWithTag("GameController").GetComponent<GameData>().GetEnemyMidpoint() - transform.position).normalized;
        transform.position += new Vector3(-speed * Time.deltaTime, 0f, 0f) + (midpointDir * clumpFactor * Time.deltaTime);  // moving left + moving some towards other enemies
        targetEnemyBuilding();
        shootAtNearbyUnit();
        targetNearbyPlayer();
        targetNearbyScrap();  // Check for nearby scrap
    }

    void chasingPlayerAction()
    {
        if (!targetExists())
        {
            toStateAdvancing();
            return;
        }
        // targetEnemyBuilding();  // Check for nearby building to attack instead of player
        moveTowardsTarget();
        if (Vector3.Distance(target.transform.position, transform.position) < attackRange)
        {
            toStateAttackingUnit(target);
        }
        if (Vector3.Distance(target.transform.position, transform.position) > sightDistance)
        {
            toStateAdvancing();
        }

        targetNearbyScrap();  // Check for nearby scrap
    }

    void attackingUnitAction()
    {
        if (!targetExists())
        {
            toStateAdvancing();
            return;
        }
        moveTowardsTarget();
        if (attackTimer - afterAttackTime < Time.time && firedRecently == false)  // if beforeAttackTime is up, attack
        {
            fireAt(target);
            firedRecently = true;
        }
        if (attackTimer < Time.time)
        {
            firedRecently = false;
            attackTimer = Time.time + beforeAttackTime + afterAttackTime;
            if (Vector3.Distance(target.transform.position, this.transform.position) > nearbyPlayerDistance)
            {
                toStateAdvancing();
            }
            if (target && target.tag == "Player")                      // Keep chasing players
            {
                toStateChasingPlayer(target);
            }
        }
    }

    void attackingBuildingAction()
    {
        if (!targetExists())
        {
            toStateAdvancing();
            return;
        }
        targetNearbyPlayer();


        if (Vector3.Distance(transform.position, target.transform.position) > nearbyBuildingDistance * 1.25f)
        {
            toStateAdvancing();
            return;
        }

        strafe();
        if (attackTimer < Time.time)
        {
            fireAt(target);
            attackTimer = buildingAttackTime + Time.time;
        }
    }

    void moveToTargetAction()
    {
        if (!targetExists())
        {
            toStateAdvancing();
            return;
        }
        moveTowardsTarget();
        targetEnemyBuilding();
        shootAtNearbyUnit();
        targetNearbyPlayer(); 
        targetNearbyScrap();
    }

    void gettingScrapAction()
    {
        if (!targetExists())
        {
            toStateAdvancing();
            return;
        }
        if (target && target.GetComponent<ConnectionHandler>().HasConnection())
        {
            target.GetComponent<BeingFetched>().ResetBeingFetchedByEnemy();
            toStateAdvancing();
            return;
        }
        if (target.tag == "Untagged")  // If a building has claimed it i.e. is pulling it in (see ScrapCollect.claimScrap)
        {
            toStateAdvancing();
            return;
        }
        moveTowardsTarget();  // Move towards scrap
    }

    void returningScrapAction()
    {
        if (!targetExists())
        {
            targetNearbyBuilding();
            return;
        }
        if (!HasConnection())
        {
            toStateAdvancing();
        }
        moveTowardsTarget();  // Move towards base
    }
    /* ========================================================*/


    /* ======================= TO STATE =======================*/
    void toStateAdvancing()
    {
        target = null;
        speed = baseSpeed;
        enemyState = EnemyState.Advancing;
    }

    void toStateChasingPlayer(GameObject player)
    {
        target = player;
        speed = baseSpeed * speedChasingFactor;
        enemyState = EnemyState.ChasingPlayer;
    }

    void toStateAttackingUnit(GameObject unit)
    {
        target = unit;
        speed = baseSpeed * speedAttackingFactor;
        attackTimer = Time.time + beforeAttackTime + afterAttackTime;
        enemyState = EnemyState.AttackingUnit;
    }

    void toStateAttackingBuilding(GameObject building)
    {
        target = building;
        speed = baseSpeed * speedAttackingBuildingFactor;
        attackTimer = Time.time + buildingAttackTime;
        enemyState = EnemyState.AttackingBuilding;
    }

    void toStateMoveToTarget(GameObject t)
    {
        target = t;
        speed = baseSpeed;
        enemyState = EnemyState.MoveToTarget;
    }

    void toStateGettingScrap(GameObject scrap)
    {
        target = scrap;
        scrap.GetComponent<BeingFetched>().SetBeingFetchedByEnemy();
        gameObject.GetComponent<GrabScrap>().scrapTarget = scrap;
        speed = baseSpeed * speedRetrievingFactor;
        enemyState = EnemyState.GettingScrap;
    }

    void toStateReturningScrap(GameObject building)
    {
        target = building;
        speed = baseSpeed * speedRetrievingFactor;
        enemyState = EnemyState.ReturningScrap;
    }
    /* ========================================================*/

    /* =================== UTILITY ACTIONS ====================*/
    void shootAtNearbyUnit()
    {
        GameObject[] units = GameObject.FindGameObjectsWithTag("Ally");
        foreach (GameObject unit in units)
        {
            if (Vector3.Distance(unit.transform.position, this.transform.position) < nearbyPlayerDistance)
            {
                toStateAttackingUnit(unit);
                return;
            }
        }
    }

    void targetNearbyPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            if (Vector3.Distance(player.transform.position, this.transform.position) < nearbyPlayerDistance)
            {
                toStateChasingPlayer(player);
                return;
            }
        }
    }

    void targetNearbyScrap()
    {
        GameObject[] allScrap = GameObject.FindGameObjectsWithTag("Scrap");
        foreach (GameObject scrap in allScrap)
        {
            if (Vector3.Distance(scrap.transform.position, this.transform.position) < nearbyScrapDistance && 
               (this.transform.position.x - scrap.transform.position.x < nearbyScrapDistance / 2))
            {
                // Don't go for scrap if it has connection or if another enemy is going for that scrap already
                if (scrap.GetComponent<ConnectionHandler>().HasConnection() || scrap.GetComponent<BeingFetched>().BeingFetchedByEnemy()) { continue; }
                toStateGettingScrap(scrap);
                return;
            }
        }
    }

    void targetNearbyBuilding()
    {
        GameObject enemyBase = GameObject.Find("Enemy Base");
        if (enemyBase) // Default return to main base
        {
            toStateReturningScrap(enemyBase);
            return;
        }

        GameObject[] buildings = GameObject.FindGameObjectsWithTag("Enemy Building");
        if (buildings.Length < 1)
        {
            GameObject obj = new GameObject();
            target = obj;
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
        toStateReturningScrap(nearestBuilding);
    }

    void targetEnemyBuilding()
    {
        GameObject[] buildings = GameObject.FindGameObjectsWithTag("Ally Building");
        foreach (GameObject building in buildings)  // Find nearest building
        {
            if (Vector3.Distance(building.transform.position, this.transform.position) < nearbyBuildingDistance)
            {
                toStateAttackingBuilding(building);
                return;
            }
            else if (building.transform.position.x > this.transform.position.x)  // Ally building is "behind" enemy
            {
                toStateMoveToTarget(building);
                return;
            }
        }

        GameObject[] buildings2 = GameObject.FindGameObjectsWithTag("Neutral Attackable");
        foreach (GameObject building in buildings2)  // Find nearest building
        {
            if (Vector3.Distance(building.transform.position, this.transform.position) < nearbyBuildingDistance)
            {
                toStateAttackingBuilding(building);
                return;
            }
        }
    }

    void moveTowardsTarget()
    {
        if (target) { transform.position = Vector3.MoveTowards(this.transform.position, target.transform.position, speed * Time.deltaTime); }
    }

    void strafe()
    {
        Vector2 dir = Vector2.Perpendicular((transform.position - target.transform.position).normalized * strafeDirection);
        if (strafeTimer < Time.time)
        {
            strafeDirection = -strafeDirection; // Switch directions
            strafeTimer = strafeTime + Time.time;
        }
        transform.position += new Vector3(dir.x * speed * Time.deltaTime, dir.y * speed * Time.deltaTime, 0);
    }

    void fireAt(GameObject target)
    {
        GameObject firedShortLaser = Instantiate(shortLaserPrefab, this.transform.position + new Vector3(0, 0.1f, 0), Quaternion.identity);
        firedShortLaser.GetComponent<EnemyShortLaser>().initEnemyShortLaser(target, this.transform.gameObject);
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            if (Vector3.Distance(transform.position, player.transform.position) < 20f)
            {
                GameObject.Find("SoundPlayer").GetComponent<SoundManager>().PlaySound("EnemyLaser");
                break;
            }
        }
    }

    bool targetExists()
    {
        if (target)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    /* ===============================================================*/



    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag == "Scrap")
        {
            if (collider.GetComponent<ConnectionHandler>().HasConnection()) { return; }
            targetNearbyBuilding();
        }
    }

    void OnCollisionEnter2D(Collision2D collider)
    {
        if (collider.gameObject.tag == "Enemy Building")
        {
            toStateAdvancing();
        }
    }


    /* ========================= PUBLIC FUNCTIONS ============================*/
    public GameObject GetTarget()
    {
        return target;
    }

    public void ResetState()
    {
        toStateAdvancing();
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
    /* ======================================================================*/
}
