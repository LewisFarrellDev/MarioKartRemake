using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CarStats : NetworkBehaviour {

    public List<CarPart> parts;
    public float speed;
    public float handling;
    public float acceleration;

    // Use this for initialization
    void Start () {
        if (isLocalPlayer)
        {
            for (int i = 0; i < 3; i++)
            {
                speed += parts[i].getSpeed();
                handling += parts[i].getHandling();
                acceleration += parts[i].getAceleration();
            }
            GameObject body = Instantiate(parts[0].partObjectPrefab, this.transform);
            body.transform.parent = this.transform;
            GameObject spoiler = Instantiate(parts[1].partObjectPrefab, body.transform.GetChild(0));
            spoiler.transform.parent = body.transform.GetChild(0);
            for (int i = 1; i < 5; i++)
            {
                GameObject Wheel = Instantiate(parts[2].partObjectPrefab, body.transform.GetChild(i));
                Wheel.transform.parent = body.transform.GetChild(i);
            }
            CmdspawnCar(this.gameObject);
        }
	}

    [Command]
    public void CmdspawnCar(GameObject player)
    {
        GameObject body = Instantiate(parts[0].partObjectPrefab, player.transform);
        NetworkServer.Spawn(body);
        RpcspawnObjects(body);


        GameObject spoiler = Instantiate(parts[1].partObjectPrefab, body.transform.GetChild(0));
        NetworkServer.Spawn(spoiler);
        RpcspawnObjects(spoiler);

        for (int i = 1; i < 5; i++)
        {
            GameObject Wheel = Instantiate(parts[2].partObjectPrefab, body.transform.GetChild(i));
            NetworkServer.Spawn(Wheel);
            RpcspawnObjects(Wheel);
        }

        

    }

    [ClientRpc]
    public void RpcspawnObjects(GameObject obj)
    {
        if (isLocalPlayer)
        {
            obj.SetActive(false);
            
        }
    }


public float getSpeed()
    {
        return speed;
    }

    public float getHandling()
    {
        return handling;
    }

    public float getAcceleration()
    {
        return acceleration;
    }
}
