using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CooldownBar : MonoBehaviour
{
    public Canvas CooldownBarCanvasPrefab;
    public Sprite CooldownBarSprite;
    Canvas CooldownBarCanvas;
    Image Cooldownbar;
    // Start is called before the first frame update
    void Start()
    {
        CooldownBarCanvas = Instantiate(CooldownBarCanvasPrefab, transform.position, Quaternion.identity, this.gameObject.transform);
        Cooldownbar = CooldownBarCanvas.transform.GetChild(0).GetChild(0).GetComponent<Image>();
        Cooldownbar.sprite = CooldownBarSprite;
    }

    // Update is called once per frame
    void Update()
    {
        Cooldownbar.fillAmount = GetComponent<Player>().GetShockwaveCooldown();
    }
}
