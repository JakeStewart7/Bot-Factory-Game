using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLaser : MonoBehaviour
{
    public float speed;
    public float laserLifeTime;
    public float damage;
    public float heal;
    public float knockback;
    public float buildingDamageFactor;
    float laserTimer = 0f;
    bool returning = false;
    bool touchingPlayer = true;

    Vector3 startPos;
    Vector3 dir;
    Vector3 velocity;
    [HideInInspector] public GameObject owner;

    public Sprite GreenLaser;

    // Parameters: Target to move towards and "owner", or object that shot the laser
    public void initPlayerLaser(Vector3 d, GameObject o)
    {
        dir = d;
        owner = o;
        transform.parent = null;
        startPos = owner.transform.position;

        velocity = (dir * speed) + (owner.GetComponent<Player>().GetVelocity());
    }
    // Start is called before the first frame update
    void Start()
    {
        transform.right = dir;  // Set direction to target
        laserTimer = laserLifeTime + Time.time;
        if (owner.name == "Player2")
        {
            this.GetComponent<SpriteRenderer>().sprite = GreenLaser;
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if (returning == false)  // Going towards target
        {
            transform.position += velocity * Time.deltaTime;
        }
        else if (returning == true)  // Returning to player
        {
            transform.position = Vector3.MoveTowards(this.transform.position, owner.transform.position, speed * Time.deltaTime);
        }

        if (laserTimer < Time.time)  // Has reached max distance
        {
            returning = true;
        }
        if (returning == true && touchingPlayer == true)  // Has returned to player
        {
            Destroy(gameObject);
        }

        if (owner.GetComponent<Player>().IsRespawning())
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag == "Enemy" || collider.tag == "Scrap")
        {
            var s = collider.gameObject.AddComponent<ShockwaveEffect>();
            s.initShockwaveEffect(transform.position, knockback);
            if (collider.tag == "Enemy")
            {
                collider.gameObject.GetComponent<Health>().Damage(damage);
            }
        }
        else if (collider.tag == "Enemy Building" || collider.tag == "Neutral Attackable")
        {
            collider.gameObject.GetComponent<Health>().Damage(damage * buildingDamageFactor);
        }
        else if (collider.gameObject == owner)  // Check for other players besides owner in next 'else if'
        {
            touchingPlayer = true;
            if (returning == false) { return; }
        }
        else if (collider.tag == "Wall" || collider.tag == "Ally Building" || collider.tag == "Ally Base" || collider.tag == "Player")
        {
            if (collider.tag == "Player")
            {
                collider.gameObject.GetComponent<Health>().Heal(heal);
            }
            // Will continue to final if/else statement
        }
        else
        {
            return;
        }

        if (returning == false)
        {
            returning = true;
        }
        else if (returning == true)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject == owner)
        {
            touchingPlayer = false;
        }
    }
}
