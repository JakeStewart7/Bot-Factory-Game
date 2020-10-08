using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnSignalIcon : MonoBehaviour
{
    public GameObject FloatingIconPrefab;
    public Sprite SignalIconSprite;
    GameObject signalIcon;
    public void SpawnSignal()
    {
        signalIcon = Instantiate(FloatingIconPrefab, transform.position + new Vector3(0, 0.7f, 0), Quaternion.identity);
        signalIcon.GetComponent<FloatingIcon>().initFloatingIcon(SignalIconSprite, 0.25f, this.gameObject);
        signalIcon.GetComponent<FloatingIcon>().DecreaseAlpha(0.3f);

    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
