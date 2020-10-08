using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Player : MonoBehaviour
{
    float speedFactor = 5f;
    Vector2 input;

    // Start is called before the first frame update
    void movementStart()
    {

    }

    // Update is called once per frame
    void movementUpdate()
    {
        input = new Vector2(Input.GetAxis("LHorizontal" + controllerNum), Input.GetAxis("LVertical" + controllerNum));
        speedCalculator(input);
    }

    private void speedCalculator(Vector3 input)
    {
        transform.position += input * speedFactor * Time.deltaTime;
    }

    public Vector3 GetVelocity()
    {
        return (input * speedFactor);
    }

    public float GetSpeed()
    {
        return speedFactor;
    }

    public void SetSpeed(float speed)
    {
        speedFactor = speed;
    }
}
