using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllyShockwave : MonoBehaviour
{
    float scaleRate;
    float maxSize;
    float minSize;
    public float damage;     // Shockwave damage
    public float heal;       // Shockwave heal
    public float knockback;  // Shockwave knockback
    public float buildingDamageFactor;
    bool retracting = false;

    
    [HideInInspector] public GameObject owner;
    
    public Sprite GreenShockwave;
    List<GameObject> targetsHit;


    public void initAllyShockwave(GameObject o)
    {
        owner = o;
        transform.parent = null;
        transform.position = owner.transform.position;
        if (owner.name == "Player2")    // Set appropriate green color for player 2
        {
            this.GetComponent<SpriteRenderer>().sprite = GreenShockwave;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        scaleRate = 2f;
        maxSize = 2.5f;
        minSize = 0.2f;

        targetsHit = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = owner.transform.position;

        if (retracting == false)  // Spin and increase in size
        {
            if (Time.deltaTime != 0) { scaleRate *= 1.04f; } 
            transform.localScale = new Vector3(transform.localScale.x + scaleRate * Time.deltaTime, transform.localScale.y + scaleRate * Time.deltaTime, transform.localScale.z);
            transform.Rotate(0, 0, -5);
        }
        else  // Spin in opposite direction and decrease in size
        {
            if (Time.deltaTime != 0) { scaleRate /= 1.04f; } 
            transform.localScale = new Vector3(transform.localScale.x - scaleRate * Time.deltaTime, transform.localScale.y - scaleRate * Time.deltaTime, transform.localScale.z);
            transform.Rotate(0, 0, 5);
        }
        
        if (transform.localScale.x > maxSize)  // Limit to growth
        {
            retracting = true;
        }
        if (transform.localScale.x < minSize)  // Destroy once too small
        {
            Destroy(gameObject);
        }

    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (targetsHit != null && targetsHit.Count > 0)
        {
            foreach (GameObject enemy in targetsHit)  // Prevent double collisions on same enemy
            {
                if (collider.gameObject == enemy) { return; }
            }
        }
        if (collider.tag == "Enemy" || collider.tag == "Scrap")
        {
            var s = collider.gameObject.AddComponent<ShockwaveEffect>();
            s.initShockwaveEffect(transform.position, knockback);
            if (collider.tag == "Enemy")
            {
                collider.gameObject.GetComponent<Health>().Damage(damage);
            }
        }
        if (collider.tag == "Player" || collider.tag == "Ally")
        {
            if (collider.name != owner.name)  // Don't heal self
            {
                collider.gameObject.GetComponent<Health>().Heal(heal);
            }
        }
        if (collider.tag == "Enemy Building" || collider.tag == "Neutral Attackable")
        {
            collider.gameObject.GetComponent<Health>().Damage(damage * buildingDamageFactor);
        }
        targetsHit.Add(collider.gameObject);
    }
}
