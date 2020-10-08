using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllyLaser : MonoBehaviour
{
    public float speed;
    public float laserDistance;
    public float damage;
    public float knockback;

    Vector3 startPos;
    public GameObject target;
    public GameObject owner;

    // Parameters: Target to move towards and "owner", or object that shot the laser
    public void initAllyLaser(GameObject t, GameObject o)
    {
        target = t;
        owner = o;
        transform.parent = null;
        startPos = owner.transform.position;
    }
    // Start is called before the first frame update
    void Start()
    {

        transform.right = target.transform.position - transform.position;  // Set direction to target
        transform.position += transform.right * 0.4f;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.right * speed * Time.deltaTime;  // Move towards target
        if (Vector3.Distance(startPos, transform.position) > laserDistance)
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
            Destroy(gameObject);
        }
        else if (collider.tag == "Enemy Building" || collider.tag == "Neutral Attackable")
        {
            collider.gameObject.GetComponent<Health>().Damage(damage);
            Destroy(gameObject);
        }
    }
}
