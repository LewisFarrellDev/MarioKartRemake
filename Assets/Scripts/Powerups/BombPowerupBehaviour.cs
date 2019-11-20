using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BombPowerupBehaviour : NetworkBehaviour
{
    public float disableDuration = 2;

    void OnTriggerEnter(Collider other)
    {
        PowerupManager powerupManager = other.GetComponent<PowerupManager>();
        if (powerupManager)
        {
            VehicleController_V2 vehicle = other.GetComponent<VehicleController_V2>();
            vehicle.StartCoroutine(vehicle.DisableVehicle(disableDuration));

            StartCoroutine(CleanUp());
        }
    }

    [Command]
    void CmdRemoveObject()
    {
        NetworkServer.Destroy(gameObject);
    }

    IEnumerator CleanUp()
    {
        AudioSource source = GetComponent<AudioSource>();
        source.Play();
        yield return new WaitForSeconds(source.clip.length);

        CmdRemoveObject();
    }
}
