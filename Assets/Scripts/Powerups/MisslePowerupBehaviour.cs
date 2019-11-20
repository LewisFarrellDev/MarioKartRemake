using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MisslePowerupBehaviour : NetworkBehaviour
{
    bool isTargetFound;
    bool isActive = true;

    public float missleSpeed = 200;
    public float disableDuration = 5;
    public float slowSpeed = 50f;
    public float lifetime = 5;

    public GameObject objectMesh;
    private GameObject target;
    int layerMask;
    float startTime;
    private GameObject ignoreObject;
    bool rotationSet;

    // Use this for initialization
    void Start()
    {
        layerMask = 1 << LayerMask.NameToLayer("Vehicle");
        layerMask = ~layerMask;
        startTime = Time.time;
        isActive = true;

    }

    void Update()
    {
        if (ignoreObject != null && !rotationSet)
        {
            transform.rotation = Quaternion.Euler(ignoreObject.transform.forward);
            rotationSet = true;
        }

        if (Time.time > startTime + lifetime && !isTargetFound)
        {
            StartCoroutine(Cleanup());
            return;
        }

        if (!isTargetFound)
        {
            if (ignoreObject != null)
            {
                transform.position = ignoreObject.transform.position + ignoreObject.transform.up * 2.5f + ignoreObject.transform.forward * 5;
                transform.rotation = ignoreObject.transform.rotation;
                objectMesh.transform.RotateAround(ignoreObject.transform.position, ignoreObject.transform.up, -missleSpeed * Time.deltaTime);
            }
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, missleSpeed * Time.deltaTime);
            transform.LookAt(target.transform);
            missleSpeed += 10 * Time.deltaTime;

            if (Vector3.Distance(transform.position, target.transform.position) < 5f && isActive)
            {
                isActive = false;
                VehicleController_V2 vehicle = target.GetComponent<VehicleController_V2>();
                vehicle.StartCoroutine(vehicle.SlowSpeed(slowSpeed, disableDuration));
                StartCoroutine(Cleanup());
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        VehicleController_V2 car = other.GetComponent<VehicleController_V2>();
        if (car == null || isTargetFound)
            return;

        if (car.gameObject == ignoreObject)
            return;

        isTargetFound = true;
        target = other.gameObject;
    }

    [ClientRpc]
    public void RpcSetIgnoreObject(GameObject obj)
    {
        ignoreObject = obj;
    }

    IEnumerator Cleanup()
    {
        GetComponentInChildren<MeshRenderer>().enabled = false;
        AudioSource source = GetComponent<AudioSource>();
        source.Play();
        yield return new WaitForSeconds(2);
        yield return new WaitForSeconds(source.clip.length);
        NetworkServer.Destroy(gameObject);
    }
}
