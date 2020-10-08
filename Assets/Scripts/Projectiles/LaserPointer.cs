using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserPointer : MonoBehaviour
{
    public Sprite GreenLaserPointer;
    GameObject owner;

    public void initLaserPointer(GameObject o)
    {
        owner = o;
        transform.parent = null;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (owner.name == "Player2")
        {
            this.GetComponent<SpriteRenderer>().sprite = GreenLaserPointer;
        }
    }
}
