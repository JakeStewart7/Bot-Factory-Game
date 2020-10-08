using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingScrap : MonoBehaviour
{
    GameObject target;
    float speed;

    public void initMovingScrap(GameObject t, float s)
    {
        target = t;
        speed = s;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (target)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);
        }
    }
}
