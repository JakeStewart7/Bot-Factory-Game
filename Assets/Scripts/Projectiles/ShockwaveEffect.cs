using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShockwaveEffect : MonoBehaviour
{

    public Vector3 ShockwavePosition;
    Vector2 dir;
    float shockwavePower;
    float deltaShockwavePower;
    Vector2 shockwavePowerTracker;

    public void initShockwaveEffect(Vector3 position, float power)
    {
        ShockwavePosition = position;
        shockwavePower = power;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        dir = (transform.position - ShockwavePosition).normalized;
        this.gameObject.GetComponent<Rigidbody2D>().velocity += shockwavePower * dir;
        shockwavePowerTracker = shockwavePower * dir;
        deltaShockwavePower = shockwavePower;
    }

    // Update is called once per frame
    void Update()
    {
        this.gameObject.GetComponent<Rigidbody2D>().velocity -= deltaShockwavePower * dir * Time.deltaTime;
        shockwavePowerTracker -= deltaShockwavePower * dir * Time.deltaTime;
        if (shockwavePowerTracker.magnitude <= 0.5f)
        {
            this.gameObject.GetComponent<Rigidbody2D>().velocity -= shockwavePowerTracker;
            Destroy(this);
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        this.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
        Destroy(this);
    }
}
