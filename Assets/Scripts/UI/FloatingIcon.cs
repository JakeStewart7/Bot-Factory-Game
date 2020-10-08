using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingIcon : MonoBehaviour
{
    GameObject owner;
    Color color;
    float offset = 0f;
    float iconSpeed;
    float alphaRate = 0.7f;

    public void initFloatingIcon(Sprite s, float size, GameObject o)
    {
        GetComponent<SpriteRenderer>().sprite = s;
        color = GetComponent<SpriteRenderer>().color;
        offset = this.transform.position.y - o.transform.position.y;
        transform.localScale *= size;
        owner = o;
    }

    // Start is called before the first frame update
    void Start()
    {
        iconSpeed = 1f;
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<SpriteRenderer>().color -= new Color(0, 0, 0, alphaRate * Time.deltaTime);
        if (GetComponent<SpriteRenderer>().color.a <= 0) { Destroy(gameObject); }  // Delete icon once invisible

        offset += iconSpeed * Time.deltaTime;
        if (owner) { transform.position = owner.transform.position + new Vector3(0, offset, 0); }

    }

    public void DecreaseAlpha(float alpha)
    {
        GetComponent<SpriteRenderer>().color -= new Color(0, 0, 0, alpha);
    }
}
