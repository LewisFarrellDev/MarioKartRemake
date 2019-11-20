// Script Created By Lewis Farrell - S15118289 //

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SetupNetworkVehicle : NetworkBehaviour
{
    public GameObject playerVehiclePrefab;

    public ColourStruct colours;
    public List<CarPart> spoilerObject = new List<CarPart>();
    public List<CarPart> wheelsObject = new List<CarPart>();
    public List<CarPart> bodyObject = new List<CarPart>();
    public List<CarPart> characterObject = new List<CarPart>();

    public int wheelID;
    public int spoilerID;
    public int bodyID;
    public int characterID;

    void Start()
    {
        if (!isLocalPlayer)
        {
            this.enabled = false;
            return;
        }

        StartCoroutine(DelayedStart());
    }

    IEnumerator DelayedStart()
    {
        yield return new WaitForSeconds(1);
        // Delay needed to wait for player ready state to change to true!
        // Unity Bug
        CmdSpawnPlayerVehicle();
    }

    [Command]
    void CmdSpawnPlayerVehicle()
    {
        // Instantiate the vehicle
        GameObject obj = Instantiate(playerVehiclePrefab, this.transform.position, this.transform.rotation);

        // Spawn the vehicle on the server assigning authority to the player who needs it
        NetworkServer.SpawnWithClientAuthority(obj, connectionToClient);

        // Update the body
        RpcUpdateBody(obj, bodyID, bodyObject[bodyID].colour);

        // Set the camera to follow the newly spawned object
        RpcSetupCameraForLocalPlayer(obj);

        //////////////////////
        /// Create Spoiler ///
        //////////////////////

        // Specificy the gameobject to which we will use for the location
        GameObject spoilerPosition = obj.transform.Find("Spoiler_Spawn").gameObject;

        // TReplace Spoiler //
        spoilerPosition.GetComponent<MeshFilter>().sharedMesh = spoilerObject[spoilerID].partObjectPrefab.GetComponent<MeshFilter>().sharedMesh;

        // Update Spoiler on clients
        RpcUpdateSpoiler(obj, spoilerID, spoilerObject[spoilerID].colour);

        // Spawn test Wheel on the client and set the parent
        //GameObject spoiler = Instantiate(spoilerObject[spoilerID].partObjectPrefab, spoilerPosition.transform.position, spoilerPosition.transform.rotation, obj.transform);

        // Spawn it on the server which makes it visible for everybody else
        //NetworkServer.Spawn(spoiler);

        // Update the position of it on the clients using the position defined earlier and the original object as the main parent
        // We have to use the original object for parent as that is the only object that exists on the client
        // if we try and use something else, it will return null and break
        //RpcUpdateSpawnedObject(spoiler, obj, spoilerPosition.transform.position, spoilerPosition.transform.rotation);

        ///// CREATE WHEELS /////
        GameObject wheelParentObject = obj.transform.Find("Wheels_Spawn").gameObject;
        for (int i = 0; i < wheelParentObject.transform.childCount; i++)
        {
            GameObject wheelChild = wheelParentObject.transform.GetChild(i).gameObject;
            wheelChild.GetComponent<MeshFilter>().sharedMesh = wheelsObject[wheelID].partObjectPrefab.GetComponent<MeshFilter>().sharedMesh;
            //GameObject wheel = Instantiate(wheelsObject[wheelID].partObjectPrefab, wheelChild.transform.position, wheelChild.transform.rotation, obj.transform);
            //NetworkServer.Spawn(wheel);
            //RpcUpdateSpawnedObject(wheel, obj, wheelChild.transform.position, wheelChild.transform.rotation);
        }

        // Update wheels on clients
        RpcUpdateWheels(obj, wheelID, wheelsObject[wheelID].colour);

        ////// Create Character //////
        GameObject characterPosition = obj.transform.Find("Character_Spawn").gameObject;
        characterPosition.GetComponent<MeshFilter>().sharedMesh = characterObject[characterID].partObjectPrefab.GetComponentInChildren<MeshFilter>().sharedMesh;
        RpcUpdateCharacter(obj, characterID);

        ////// CONFIGURE CAR STATS //////

        // Set the base stats
        obj.GetComponent<VehicleController_V2>().speed = bodyObject[bodyID].speed;
        obj.GetComponent<VehicleController_V2>().acceleration = bodyObject[bodyID].acceleration;
        obj.GetComponent<VehicleController_V2>().handling = bodyObject[bodyID].handling;

        obj.GetComponent<VehicleController_V2>().speed += wheelsObject[wheelID].getSpeed() + spoilerObject[spoilerID].getSpeed();
        obj.GetComponent<VehicleController_V2>().acceleration += wheelsObject[wheelID].getAceleration() + spoilerObject[spoilerID].getAceleration();
        obj.GetComponent<VehicleController_V2>().handling += wheelsObject[wheelID].getHandling() + spoilerObject[spoilerID].getHandling();

        // Send to clients
        RpcUpdateVehicleStats(obj, obj.GetComponent<VehicleController_V2>().speed, obj.GetComponent<VehicleController_V2>().acceleration, obj.GetComponent<VehicleController_V2>().handling);
    }

    [ClientRpc]
    void RpcUpdateBody(GameObject obj, int id, ColourStruct colour)
    {
        // Chaneg mesh
        obj.transform.Find("Mesh_Vehicle").gameObject.GetComponent<MeshFilter>().sharedMesh = bodyObject[id].partObjectPrefab.GetComponent<MeshFilter>().sharedMesh;

        if (obj.transform.Find("Mesh_Vehicle").gameObject.GetComponent<Renderer>() == null)
            return;

        MaterialPropertyBlock props = new MaterialPropertyBlock();
        props.SetColor("_Color_Red", colour.redChannel);
        props.SetColor("_Color_Green", colour.greenChannel);
        props.SetColor("_Color_Blue", colour.blueChannel);
        props.SetColor("_Color_Alpha", colour.alphaChannel);
        obj.transform.Find("Mesh_Vehicle").GetComponent<Renderer>().SetPropertyBlock(props);
    }

    [ClientRpc]
    void RpcUpdateSpoiler(GameObject obj, int id, ColourStruct colour)
    {
        obj.transform.Find("Spoiler_Spawn").gameObject.GetComponent<MeshFilter>().sharedMesh = spoilerObject[id].partObjectPrefab.GetComponent<MeshFilter>().sharedMesh;

        if (obj.transform.Find("Spoiler_Spawn").gameObject.GetComponent<Renderer>() == null)
            return;

        MaterialPropertyBlock props = new MaterialPropertyBlock();
        props.SetColor("_Color_Red", colour.redChannel);
        props.SetColor("_Color_Green", colour.greenChannel);
        props.SetColor("_Color_Blue", colour.blueChannel);
        props.SetColor("_Color_Alpha", colour.alphaChannel);
        obj.transform.Find("Spoiler_Spawn").GetComponent<Renderer>().SetPropertyBlock(props);
    }

    [ClientRpc]
    void RpcUpdateCharacter(GameObject obj, int id)
    {
        obj.transform.Find("Character_Spawn").gameObject.GetComponent<MeshFilter>().sharedMesh = characterObject[id].partObjectPrefab.GetComponentInChildren<MeshFilter>().sharedMesh;
    }

    [ClientRpc]
    void RpcUpdateWheels(GameObject obj, int id, ColourStruct colour)
    {
        GameObject wheelParentObject = obj.transform.Find("Wheels_Spawn").gameObject;
        for (int i = 0; i < wheelParentObject.transform.childCount; i++)
        {
            GameObject wheelChild = wheelParentObject.transform.GetChild(i).gameObject;
            wheelChild.GetComponent<MeshFilter>().sharedMesh = wheelsObject[id].partObjectPrefab.GetComponent<MeshFilter>().sharedMesh;

            // Change the colour
            if (wheelChild.GetComponent<Renderer>() == null)
                return;

            MaterialPropertyBlock props = new MaterialPropertyBlock();
            props.SetColor("_Color_Red", colour.redChannel);
            props.SetColor("_Color_Green", colour.greenChannel);
            props.SetColor("_Color_Blue", colour.blueChannel);
            props.SetColor("_Color_Alpha", colour.alphaChannel);
            wheelChild.GetComponent<Renderer>().SetPropertyBlock(props);
        }
    }

    [ClientRpc]
    void RpcUpdateSpawnedObject(GameObject spawned, GameObject parent, Vector3 position, Quaternion rotation)
    {
        spawned.transform.parent = parent.transform;
        spawned.transform.position = position;
        spawned.transform.rotation = rotation;
    }

    [ClientRpc]
    void RpcUpdateVehicleColour(GameObject obj, ColourStruct colours)
    {
        // Update the vehicle colour for all the other players
        obj.GetComponentInChildren<ColourPicker>().colours = colours;
    }

    [ClientRpc]
    void RpcSetupCameraForLocalPlayer(GameObject obj)
    {
        if (isLocalPlayer)
            Camera.main.GetComponent<CameraFollow>().SetupTarget(obj.transform, obj.transform.Find("CamPosition").gameObject.transform);
    }

    [ClientRpc]
    void RpcUpdateVehicleStats(GameObject obj, float speed, float acceleration, float handling)
    {
        if (isLocalPlayer)
        {
            obj.GetComponent<VehicleController_V2>().speed = speed;
            obj.GetComponent<VehicleController_V2>().acceleration = acceleration;
            obj.GetComponent<VehicleController_V2>().handling = handling;
        }
    }
}
