using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioClip AllyLaserSound;
    public AudioClip EnemyLaserSound;
    public AudioClip PlayerLaserSound;
    public AudioClip PlayerShockwaveSound;
    public AudioClip PlayerSignalSound;
    public AudioClip SignalResponseSound;
    public AudioClip PlayerDashSound;
    public AudioClip UnitExplosionSound;
    public AudioClip BuildingExplosionSound;
    public AudioClip PlayerExplosionSound;
    public AudioClip RockDestructionSound;
    
    private AudioSource audioSource;

    float baseVolume = 0.10f;

    float AllyLaserVolumeFactor = 1f;
    float EnemyLaserVolumeFactor = 1.5f;
    float PlayerLaserVolumeFactor = 1f;
    float PlayerShockwaveVolumeFactor = 7f;
    float PlayerSignalVolumeFactor = 1f;
    float SignalResponseVolumeFactor = 1f;
    float PlayerDashVolumeFactor = 1.5f;
    float UnitExplosionVolumeFactor = 1f;
    float BuildingExplosionVolumeFactor = 1f;
    float PlayerExplosionVolumeFactor = 1f;
    float RockDestructionVolumeFactor = 8f;


    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlaySound(string sound)
    {
        switch(sound)
        {
            // Player sounds
            case "PlayerLaser":
                audioSource.PlayOneShot(PlayerLaserSound, PlayerLaserVolumeFactor * baseVolume);
                break;
            case "PlayerShockwave":
                audioSource.PlayOneShot(PlayerShockwaveSound, PlayerShockwaveVolumeFactor * baseVolume);
                break;
            case "PlayerSignal":
                audioSource.PlayOneShot(PlayerSignalSound, PlayerSignalVolumeFactor * baseVolume);
                break;
            case "PlayerDash":
                audioSource.PlayOneShot(PlayerDashSound, PlayerDashVolumeFactor * baseVolume);
                break;
            case "PlayerExplosion":
                audioSource.PlayOneShot(PlayerExplosionSound, PlayerExplosionVolumeFactor * baseVolume);
                break;
            
            // Unit sounds
            case "AllyLaser":
                audioSource.PlayOneShot(AllyLaserSound, AllyLaserVolumeFactor * baseVolume);
                break;
            case "EnemyLaser":
                audioSource.PlayOneShot(EnemyLaserSound, EnemyLaserVolumeFactor * baseVolume);
                break;
            case "SignalResponse":
                audioSource.PlayOneShot(SignalResponseSound, SignalResponseVolumeFactor * baseVolume);
                break;
            case "UnitExplosion":
                audioSource.PlayOneShot(UnitExplosionSound, UnitExplosionVolumeFactor * baseVolume);
                break;
            case "BuildingExplosion":
                audioSource.PlayOneShot(BuildingExplosionSound, BuildingExplosionVolumeFactor * baseVolume);
                break;
            case "RockDestruction":
                audioSource.PlayOneShot(RockDestructionSound, RockDestructionVolumeFactor * baseVolume);
                break;
                
            default:
                break;
        }
    }
}
