using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSignal : MonoBehaviour
{
    float speed;
    float alphaRate = 0.6f;


    Vector3 startPos;
    Vector3 dir;
    [HideInInspector] public GameObject owner;

    // public Sprite GreenLaser;

    // Parameters: Direction to move towards and "owner", or object that shot the laser
    public void initPlayerSignal(Vector3 d, GameObject o)
    {
        dir = d;
        owner = o;
        transform.parent = null;
        startPos = owner.transform.position;
    }
    // Start is called before the first frame update
    void Start()
    {
        speed = 10.5f;
        transform.right = dir;  // Set direction to target

        //if (owner.name == "Player2")
        //{
        //    this.GetComponent<SpriteRenderer>().sprite = GreenLaser;
        //}
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += dir * speed * Time.deltaTime;  // Move towards target
        transform.Rotate(0, 0, -420 * Time.deltaTime);
        GetComponent<SpriteRenderer>().color -= new Color(0, 0, 0, alphaRate * Time.deltaTime);
        if (GetComponent<SpriteRenderer>().color.a <= 0)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag == "Ally")
        {
            alphaRate *= 7f;
            speed /= 2;
            Destroy(GetComponent<BoxCollider2D>());
        }
    }

    public GameObject GetOwner()
    {
        return owner;
    }
}
