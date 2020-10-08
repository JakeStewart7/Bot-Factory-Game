using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    public float maxHealth = 100f;
    float health;

    public Canvas healthCanvasPrefab;
    public GameObject FloatingIconPrefab;
    public Sprite HealingIconSprite;
    Canvas healthCanvas;
    Image healthBar;

    Vector2 objectSize;

    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
        setColliderSize();
        healthCanvas = Instantiate(healthCanvasPrefab, transform.position, Quaternion.identity, null);
        healthCanvas.transform.SetParent(GameObject.Find("CanvasHolder").transform);

        healthCanvas.transform.localScale = new Vector3(objectSize.x * 1.2f * transform.localScale.x,  // Set height/width to scale
                                                        objectSize.y * 0.02f + 0.2f, 1); 
        healthBar = healthCanvas.transform.GetChild(0).GetChild(0).GetComponent<Image>();  // Set actual health BAR reference (colored part)
        setColor();  // Set health bar color based on tag
    }

    // Update is called once per frame
    void Update()
    {
        healthCanvas.transform.position = transform.position + new Vector3(0, ((objectSize.y * transform.localScale.y) / 1.8f) + 0.1f, 0);
    }

    // Takes a change in health, and a bool if it is damage or not (heals)
    public void Damage(float damage)
    {
        health -= damage;
        if (health <= 0)  // If out of health, destroy unit and replace with scrap
        {
            var s = gameObject.GetComponent<UnitDestructor>();  // Check for unit destructor script
            if (s)
            {
                s.DestroyUnit();
            }
            else    // If no destructor script, simply destroy the object
            {
                Destroy(gameObject);
            }
        }
        else
        {
            updateHealthBar();
        }
    }

    public void Heal(float heal)
    {
        if (health >= maxHealth) { return; }  // If full health, do nothing

        if (health + heal > maxHealth)
        {
            health = maxHealth;
            updateHealthBar();
        }
        else
        {
            health += heal;
            updateHealthBar();
        }
        GameObject healingIcon = Instantiate(FloatingIconPrefab, transform.position, Quaternion.identity);
        healingIcon.GetComponent<FloatingIcon>().initFloatingIcon(HealingIconSprite, 0.25f, this.gameObject);
    }

    public void FullHeal()
    {
        Heal(maxHealth - health);
    }

    void setColor()
    {
        if (gameObject.name == "Player2")
        {
            healthBar.color = new Color(0, 255, 0);
        }
        else if (gameObject.tag == "Enemy" || gameObject.tag == "Enemy Building")
        {
            healthBar.color = new Color(255, 0, 0);
        }
        else if (gameObject.tag == "Neutral Attackable" || gameObject.tag == "Neutral Building")
        {
            healthBar.color = new Color (0.75f, 0.75f, 0.75f);
        }

    }

    void updateHealthBar()
    {
        healthBar.fillAmount = (float)health / maxHealth;
    }

    public void DestroyHealthBar()
    {
        Destroy(healthCanvas.gameObject);
    }

    void setColliderSize()
    {
        if (this.GetComponent<BoxCollider2D>() != null)
        {
            objectSize = this.GetComponent<BoxCollider2D>().size;
        }
        else if (this.GetComponent<CircleCollider2D>() != null)
        {
            objectSize = new Vector3 (this.GetComponent<CircleCollider2D>().radius * 1.9f, this.GetComponent<CircleCollider2D>().radius * 1.9f);
        }
        else
        {
            Debug.LogError("Health: No valid collider");
        }
    }
}
