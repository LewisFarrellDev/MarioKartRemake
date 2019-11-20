using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[CreateAssetMenu(fileName = "BombPowerup", menuName = "Powerups/BombPowerup", order = 1)]
public class BombPowerup : BasePowerup
{
    public GameObject bombPrefab;

    public override void ActivatePowerup(GameObject playerRef)
    {
        GameObject spawnedObject = Instantiate(bombPrefab, playerRef.transform.position - playerRef.transform.forward * 10 - playerRef.transform.up * 1.5f, playerRef.transform.rotation);
        NetworkServer.Spawn(spawnedObject);
    }
}
