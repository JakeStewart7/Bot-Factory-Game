using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Player : MonoBehaviour
{
    public GameObject LaserPointerPrefab;
    GameObject laserPointer;

    float laserPointerDistance;
    Vector3 dir;

    // Start is called before the first frame update
    void laserPointerStart()
    {
        laserPointer = Instantiate(LaserPointerPrefab, transform.position, Quaternion.identity);
        laserPointer.GetComponent<LaserPointer>().initLaserPointer(this.gameObject);

        laserPointerDistance = laserPointer.GetComponent<SpriteRenderer>().bounds.size.x / 2;
        dir = new Vector3(1, 0, 0);
    }

    // Update is called once per frame
    void laserPointerUpdate()
    {
        // Check that right thumb stick is being used
        if (Mathf.Abs(Input.GetAxis("RHorizontal" + controllerNum)) > 0.2f || Mathf.Abs(Input.GetAxis("RVertical" + controllerNum)) > 0.2f)
        {
            dir = new Vector3(Input.GetAxis("RHorizontal" + controllerNum), Input.GetAxis("RVertical" + controllerNum)).normalized;
        }
        laserPointer.transform.position = transform.position + laserPointerDistance * dir;
        laserPointer.transform.right = dir;
    }

    Vector3 getPointerDir()
    {
        return dir;
    }
}
