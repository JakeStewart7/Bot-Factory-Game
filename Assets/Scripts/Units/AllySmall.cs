// AllySmall is produced from Player Factories and automatically shoot enemies nearby.
// It is either advancing towards the enemy's side, following the player, or attacking their base.
// Regardless of movement, it will fire at nearby enemies.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllySmall : MonoBehaviour
{
    public enum AllyState {Advancing, FollowingPlayer, Defending, AttackingBuilding, MoveToTarget};
    public AllyState allyState;  // PUBLIC FOR TESTING TO SEE IN INSPECTOR


    public float baseSpeed = 1f;        // AllySmall speed
    float speed;
    float maxSpeed = 2.8f;
    float nearbyDistance = 9f;  // Distance that defines a target as nearby

    float cooldownTimer = 0;
    public float AttackCooldown = 1.5f;  // Laser cooldown

    GameObject followTarget;
    public GameObject laserPrefab;

    float defendDirectionTimer = 0f;
    float defendDirectionTime = 3f;   // Defend direction cooldown
    int defendDirection = 1;

    float attackDirectionTimer = 0f;
    float attackDirectionTime = 0.7f;
    Vector2[] directions;
    Vector2 direction;
    int directionIndex = 0;

    float speedAdvancingFactor = 1f;
    float speedFollowingPlayerFactor = 0.8f;
    float speedDefendingFactor = 1f;
    float speedAttackingBuildingFactor = 1f;
    float attackSpeedFollowingPlayerFactor = 0.6f;

    // Start is called before the first frame update
    void Start()
    {
        speed = baseSpeed;
        directions = new Vector2[] { new Vector2 (1f, 1f), new Vector2 (1f, -1f), new Vector2 (-1f, -1f), new Vector2 (-1f, 1f) };
        direction = new Vector2 (1f, 1f);
        toStateAdvancing();
    }

    // Update is called once per frame
    void Update()
    {
        switch(allyState)
        {
            case AllyState.Advancing:
                advancingAction();
                break;
            case AllyState.FollowingPlayer:
                followingPlayerAction();
                break;
            case AllyState.Defending:
                defendingAction();
                break;
            case AllyState.AttackingBuilding:
                attackingBuildingAction();
                break;
            case AllyState.MoveToTarget:
                moveToTargetAction();
                break;
        }
    }


    /* =======================   Actions   ======================= */
    void advancingAction()
    {
        transform.position += new Vector3(speed * Time.deltaTime, 0f, 0f);  // Moving right
        targetNearbyBuilding();
        targetNearbyEnemy();
    }

    void followingPlayerAction()
    {
        float tempSpeed = speed * (Vector3.Distance(followTarget.transform.position, transform.position) - 1f);
        if (tempSpeed > maxSpeed) { tempSpeed = maxSpeed; }  // Cap out max follow speed

        transform.position = Vector3.MoveTowards(this.transform.position, followTarget.transform.position, tempSpeed * Time.deltaTime);
        targetNearbyEnemy();
        fireAtNearbyBuilding();
    }

    void defendingAction()
    {
        if (defendDirectionTimer < Time.time)
        {
            defendDirection = -defendDirection;
            defendDirectionTimer = defendDirectionTime + Time.time;
        }
        transform.position += new Vector3(0f, speed * defendDirection * Time.deltaTime, 0f);
        targetNearbyEnemy();
        targetNearbyBuilding();
    }

    void attackingBuildingAction()
    {
        if (!followTarget || Vector3.Distance(this.transform.position, followTarget.transform.position) > nearbyDistance * 1.25f)
        {
            toStateAdvancing();
            return;
        }
        if (attackDirectionTimer < Time.time)
        {
            setDirection();
            attackDirectionTimer = attackDirectionTime + Time.time;
        }
        transform.position += new Vector3(direction.x * speed * Time.deltaTime, direction.y * speed * Time.deltaTime, 0);

        targetNearbyEnemy();  // If nearby enemy, will fire at them instead of building
        fireAt(followTarget); // If no nearby enemies, will fire at building
    }

    void moveToTargetAction()
    {
        moveTowardsTarget();
        targetNearbyEnemy();
        targetNearbyBuilding();
    }
    /* =========================================================== */


    /* =======================   toState   ======================= */
    void toStateAdvancing()
    {
        followTarget = null;
        speed = baseSpeed * speedAdvancingFactor;
        GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
        allyState = AllyState.Advancing;
    }

    void toStateFollowingPlayer(GameObject target)
    {
        followTarget = target;
        speed = baseSpeed * speedFollowingPlayerFactor;
        GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
        allyState = AllyState.FollowingPlayer;
    }

    void toStateDefending()
    {
        followTarget = null;
        // Divide defendDirectionTime by 2 so wherever it is upon entering this mode is the center position of its patrol path
        defendDirectionTimer = (defendDirectionTime / 2) + Time.time;
        speed = baseSpeed * speedDefendingFactor;
        GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
        allyState = AllyState.Defending;
    }

    void toStateAttackingBuilding(GameObject building)
    {
        direction = new Vector2 (1f, 1f);
        directionIndex = 0;
        attackDirectionTimer = attackDirectionTime + Time.time;

        followTarget = building;
        speed = baseSpeed * speedAttackingBuildingFactor;
        GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
        allyState = AllyState.AttackingBuilding;
    }

    void toStateMoveToTarget(GameObject t)
    {
        followTarget = t;
        speed = baseSpeed;
        allyState = AllyState.MoveToTarget;
    }
    /* =========================================================== */


    void targetNearbyEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            if (Vector3.Distance(enemy.transform.position, this.transform.position) < nearbyDistance)
            {
                fireAt(enemy);
            }
        }
    }

    void targetNearbyBuilding()
    {
        GameObject[] buildings = GameObject.FindGameObjectsWithTag("Enemy Building");
        foreach (GameObject building in buildings)
        {
            if (Vector3.Distance(building.transform.position, this.transform.position) < nearbyDistance && allyState != AllyState.FollowingPlayer)
            {
                toStateAttackingBuilding(building);
                return;
            }
            else if (building.transform.position.x < this.transform.position.x)  // Enemy building is "behind" ally
            {
                toStateMoveToTarget(building);
                return;
            }
        }

        GameObject[] buildings2 = GameObject.FindGameObjectsWithTag("Neutral Attackable");
        foreach (GameObject building in buildings2)
        {
            if (Vector3.Distance(building.transform.position, this.transform.position) < nearbyDistance && allyState != AllyState.FollowingPlayer)
            {
                toStateAttackingBuilding(building);
            }
        }
    }

    void fireAt(GameObject target)
    {
        if (cooldownTimer > Time.time) { return; }  // Only fire when cooldown is ready

        GameObject firedLaser = Instantiate(laserPrefab, this.transform.position + new Vector3(0, 0.2f, 0), Quaternion.identity);
        firedLaser.GetComponent<AllyLaser>().initAllyLaser(target, this.transform.gameObject);
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            if (Vector3.Distance(transform.position, player.transform.position) < 20f)
            {
                GameObject.Find("SoundPlayer").GetComponent<SoundManager>().PlaySound("AllyLaser");
                break;
            }
        }

        if (allyState == AllyState.FollowingPlayer)
        {
            cooldownTimer = Time.time + (AttackCooldown / attackSpeedFollowingPlayerFactor);
        }
        else
        {
            cooldownTimer = Time.time + AttackCooldown;
        }
    }

    void fireAtNearbyBuilding()
    {
        GameObject[] buildings = GameObject.FindGameObjectsWithTag("Enemy Building");
        foreach (GameObject building in buildings)
        {
            if (Vector3.Distance(building.transform.position, this.transform.position) < 
                    nearbyDistance + building.GetComponent<SpriteRenderer>().sprite.bounds.size.x / 10)  // Look for edge, not center of building
            {
                fireAt(building);
            }
        }
    }

    void setDirection()
    {
        directionIndex++;
        if (directionIndex >= directions.Length)
        {
            directionIndex = 0;
        }
        direction = directions[directionIndex];
    }

    void moveTowardsTarget()
    {
        if (followTarget) { transform.position = Vector3.MoveTowards(this.transform.position, followTarget.transform.position, speed * Time.deltaTime); }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag == "Signal")
        {
            GetComponent<SpawnSignalIcon>().SpawnSignal();
            GameObject.Find("SoundPlayer").GetComponent<SoundManager>().PlaySound("SignalResponse");
            if (allyState == AllyState.Advancing || allyState == AllyState.MoveToTarget)  // Advancing/MoveToTarget state goes to FollowingPlayer state
            {
                toStateFollowingPlayer(collider.GetComponent<PlayerSignal>().GetOwner());
            }
            else if (allyState == AllyState.FollowingPlayer)    // FollowingPlayer state goes to defending state
            {
                toStateDefending();
            }
            else if (allyState == AllyState.Defending)          // Defending state goes to Advancing state
            {
                toStateAdvancing();
            }
            else if (allyState == AllyState.AttackingBuilding)  // AttackingBuilding state goes to FollowingPlayer state
            {
                toStateFollowingPlayer(collider.GetComponent<PlayerSignal>().GetOwner());
            }
        }
    }

    public GameObject GetTarget()
    {
        return followTarget;
    }

    public void ResetState()
    {
        toStateAdvancing();
    }
}
