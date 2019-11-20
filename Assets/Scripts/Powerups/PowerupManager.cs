// Script Created By Lewis Farrell - S15118289 //

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PowerupManager : NetworkBehaviour
{
    public BasePowerup currentPowerup;
    bool isHoldingPowerup = false;
    private int numUses = 0;
    public GameObject powerupUI;
    float lastActivation = 0;
    public float fireRate = 0.5f;
    public AudioSource audioSource;

    void Start()
    {
        if (!hasAuthority)
        {
            powerupUI.SetActive(false);
        }
        else
        {
            powerupUI.GetComponentInChildren<Image>().enabled = false;
        }
    }

    void Update()
    {
        if (Input.GetButtonDown("ActivatePowerup"))
        {
            if (currentPowerup != null && isHoldingPowerup)
            {
                if (hasAuthority)
                {
                    if (Time.time > lastActivation + fireRate)
                    {
                        lastActivation = Time.time;
                        if (currentPowerup.requiresNetwork)
                            CmdActivatePowerup();
                        else
                            currentPowerup.ActivatePowerup(gameObject);

                        audioSource.Play();
                        numUses++;
                        if (numUses >= currentPowerup.numUses)
                        {
                            RemovePowerup();
                        }
                    }
                }
            }
        }
    }

    public void SetCurrentPowerup(BasePowerup powerup)
    {
        if (!isHoldingPowerup)
        {
            print(powerup.name);
            currentPowerup = powerup;
            isHoldingPowerup = true;
            CmdUpdatePowerup(currentPowerup.name);
            powerupUI.GetComponentInChildren<Image>().sprite = currentPowerup.powerupSprite;
            powerupUI.GetComponentInChildren<Image>().enabled = true;
        }
    }

    public void RemovePowerup()
    {
        isHoldingPowerup = false;
        currentPowerup = null;
        numUses = 0;
        powerupUI.GetComponentInChildren<Image>().enabled = false;
    }

    [Command]
    void CmdUpdatePowerup(string powerupName)
    {
        currentPowerup = null;
        currentPowerup = Resources.Load<BasePowerup>(powerupName);
    }

    [Command]
    public void CmdActivatePowerup()
    {
        currentPowerup.ActivatePowerup(gameObject);
    }

    [Command]
    public void CmdDestroyObject(GameObject obj)
    {
        NetworkServer.Destroy(obj);
    }
}
