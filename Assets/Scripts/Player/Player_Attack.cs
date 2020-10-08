using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Player : MonoBehaviour
{
    float laserCooldownTimer = 0f;  // Counter for laser cooldown
    float shockwaveCooldownTimer = 0f; // Counter for shockwave cooldown
    float signalCooldownTimer = 0f; // Counter for signal cooldown
    float dashCooldownTimer = 0f;
    float dashTimer = 0f;
    
    public float LaserCooldown = 0.8f;
    public float ShockwaveCooldown = 1f;
    public float SignalCooldown = 0.4f;
    public float DashCooldown = 2f;
    public float DashTime = 0.3f;
    public float DashSpeedFactor = 3f;
    float scrapDetachForce = 2f;

    float timeSinceUsedCooldown;
    bool recharging = false;
    bool dashing = false;

    public GameObject LaserPrefab;
    public GameObject ShockwavePrefab;
    public GameObject SignalPrefab;

    // Start is called before the first frame update
    void attackStart()
    {
        timeSinceUsedCooldown = ShockwaveCooldown;
    }

    // Update is called once per frame
    void attackUpdate()
    {
        if (Input.GetAxis("PrimaryFire" + controllerNum) > 0.6f && laserCooldownTimer <= Time.time)
        {
            GameObject laser = Instantiate(LaserPrefab, transform.position, Quaternion.identity);
            Vector3 laserDirection = getPointerDir();
            laser.GetComponent<PlayerLaser>().initPlayerLaser(laserDirection, this.gameObject);
            GameObject.Find("SoundPlayer").GetComponent<SoundManager>().PlaySound("PlayerLaser");
            laserCooldownTimer = Time.time + LaserCooldown;
        }
        if (Input.GetAxis("SecondaryFire" + controllerNum) > 0.6f && shockwaveCooldownTimer <= Time.time)
        {
            GameObject shockwave = Instantiate(ShockwavePrefab, transform.position, Quaternion.identity);
            shockwave.GetComponent<AllyShockwave>().initAllyShockwave(this.gameObject);
            GameObject.Find("SoundPlayer").GetComponent<SoundManager>().PlaySound("PlayerShockwave");
            shockwaveCooldownTimer = Time.time + ShockwaveCooldown;
            timeSinceUsedCooldown = 0f;
            recharging = true;
        }
        if (Input.GetButton("PrimarySpecial" + controllerNum) && signalCooldownTimer <= Time.time)
        {
            GameObject signal = Instantiate(SignalPrefab, transform.position, Quaternion.identity);
            Vector3 signalDirection = getPointerDir();
            signal.GetComponent<PlayerSignal>().initPlayerSignal(signalDirection, this.gameObject);
            GameObject.Find("SoundPlayer").GetComponent<SoundManager>().PlaySound("PlayerSignal");
            signalCooldownTimer = Time.time + SignalCooldown;
        }
        if (Input.GetButton("SecondarySpecial" + controllerNum) && dashCooldownTimer <= Time.time)
        {
            SetSpeed(GetSpeed() * DashSpeedFactor);
            GameObject.Find("SoundPlayer").GetComponent<SoundManager>().PlaySound("PlayerDash");
            dashCooldownTimer = Time.time + DashCooldown;
            dashTimer = Time.time + DashTime;
            dashing = true;
        }
        if (Input.GetButton("DropButton" + controllerNum) && GetComponent<GrabScrap>().HasScrapConnection())
        {
            pullTarget(GetComponent<GrabScrap>().scrap, scrapDetachForce);
            GetComponent<GrabScrap>().DestroyScrapConnection();
        }
        if (recharging == true)
        {
            timeSinceUsedCooldown += Time.deltaTime;
            if (timeSinceUsedCooldown > ShockwaveCooldown)
            {
                timeSinceUsedCooldown = ShockwaveCooldown;
                recharging = false;
            }
        }
        if (dashing == true)
        {
            if (dashTimer < Time.time)
            {
                SetSpeed(GetSpeed() / DashSpeedFactor);
                dashing = false;
            }
        }
    }

    void pullTarget(GameObject target, float force)
    {
        // Push from the opposite side
        Vector3 v = target.transform.position - transform.position;
        Vector3 targetPosition = transform.position + (v * 2);
        
        var s = target.AddComponent<ShockwaveEffect>();
        s.initShockwaveEffect(targetPosition, force);
    }

    public float GetShockwaveCooldown()
    {
        return timeSinceUsedCooldown / ShockwaveCooldown;
    }
}
