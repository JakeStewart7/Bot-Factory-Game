using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeingFetched : MonoBehaviour
{
    bool beingFetchedByAlly = false;
    bool beingFetchedByEnemy = false;

    public bool BeingFetchedByAlly()
    {
        return beingFetchedByAlly;
    }

    public bool BeingFetchedByEnemy()
    {
        return beingFetchedByEnemy;
    }

    public void SetBeingFetchedByAlly()
    {
        beingFetchedByAlly = true;
    }

    public void ResetBeingFetchedByAlly()
    {
        beingFetchedByAlly = false;
    }

    public void SetBeingFetchedByEnemy()
    {
        beingFetchedByEnemy = true;
    }

    public void ResetBeingFetchedByEnemy()
    {
        beingFetchedByEnemy = false;
    }
}
