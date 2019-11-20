using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[CreateAssetMenu(fileName = "MisslePowerup", menuName = "Powerups/MisslePowerup", order = 1)]
public class MisslePowerup : BasePowerup
{
    public GameObject misslePrefab;
    public override void ActivatePowerup(GameObject playerRef)
    {
        GameObject spawnedObject = Instantiate(misslePrefab, playerRef.transform.position + playerRef.transform.forward * 10 + playerRef.transform.up * 5.5f, playerRef.transform.rotation);
        NetworkServer.Spawn(spawnedObject);
        spawnedObject.GetComponent<MisslePowerupBehaviour>().RpcSetIgnoreObject(playerRef);
    }
}
