// Script Created By Lewis Farrell - S15118289 //

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupBlock : MonoBehaviour
{
    public List<BasePowerup> powerupList = new List<BasePowerup>();
    public float powerupCooldown = 10;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void OnTriggerEnter(Collider other)
    {
        PowerupManager powerupManager = other.GetComponent<PowerupManager>();

        if (powerupManager)
        {
            int randomNumber = Mathf.RoundToInt(Random.Range(0, powerupList.Count));
            BasePowerup randomPowerup = powerupList[randomNumber];
            powerupManager.SetCurrentPowerup(randomPowerup);
            
            if (audioSource)
            {
                audioSource.Play();
            }

            StartCoroutine(DisablePowerup());
        }
    }

    IEnumerator DisablePowerup()
    {
        BoxCollider collider = GetComponent<BoxCollider>();
        MeshRenderer renderer = GetComponentInChildren<MeshRenderer>();

        if (collider != null)
            collider.enabled = false;

        if (renderer != null)
            renderer.enabled = false;

        yield return new WaitForSeconds(powerupCooldown);


        if (collider != null)
            collider.enabled = true;

        if (renderer != null)
            renderer.enabled = true;
    }
}
