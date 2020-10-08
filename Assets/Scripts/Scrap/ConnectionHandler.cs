// Keep track of whether or not Scrap has a connection to limit to 1 connection

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionHandler : MonoBehaviour
{
    bool connection = false;
    GameObject owner;
    public void Connect(GameObject o)
    {
        owner = o;
        if (connection == false)
        {
            connection = true;
        }
    }

    public void Disconnect()
    {
        if (connection == true)
        {
            connection = false;
        }
        owner = null;
    }
    
    public bool HasConnection()
    {
        return connection;
    }

    public GameObject GetOwner()
    {
        return owner;
    }
}
