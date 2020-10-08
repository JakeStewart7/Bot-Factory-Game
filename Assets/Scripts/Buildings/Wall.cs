using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag == "Scrap")
        {
           var s = collider.GetComponent<ShockwaveEffect>();
           if (s) { Destroy(s); }
           collider.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
        }
    }
}
