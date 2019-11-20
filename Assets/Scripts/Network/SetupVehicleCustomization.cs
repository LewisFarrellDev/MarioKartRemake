// Script Created By Lewis Farrell - S15118289 //

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class SetupVehicleCustomization : NetworkBehaviour
{
    public ColourStruct vehicleColour = new ColourStruct(Color.red, Color.green, Color.blue, Color.black);
    public int wheelID = 0;
    public int spoilerID = 0;
    public int bodyID = 0;
    public int characterID = 0;
    public GameObject spoilerButton;
    public GameObject wheelButton;
    public GameObject bodyButton;
    public GameObject characterButton;
    public Slider speedSlider;
    public Slider accelerationSlider;
    public Slider handlingSlider;
    public List<CarPart> spoilers = new List<CarPart>();
    public List<CarPart> wheels = new List<CarPart>();
    public List<CarPart> bodys = new List<CarPart>();
    public List<CarPart> characters = new List<CarPart>();

    private GameObject previewCar;

    // Use this for initialization
    void Start()
    {
        OnValidate();

        spoilerButton.GetComponent<Image>().sprite = spoilers[spoilerID].partSprite;
        wheelButton.GetComponent<Image>().sprite = wheels[wheelID].partSprite;
        bodyButton.GetComponent<Image>().sprite = bodys[bodyID].partSprite;
        characterButton.GetComponent<Image>().sprite = characters[characterID].partSprite;

        if (isLocalPlayer)
        {
            UpdatePreviewCar();
            UpdateSliders();
        }
    }

    void UpdatePreviewCar()
    {
        // Find the preview Car
        previewCar = GameObject.Find("PreviewCart");

        // Update car colours
        GameObject carMesh = previewCar.transform.Find("Mesh_Vehicle").gameObject;
        ColourStruct colours = bodys[bodyID].colour;
        UpdatePartColour(carMesh, colours.redChannel, colours.blueChannel, colours.greenChannel, colours.alphaChannel);

        // Find the spoiler part
        GameObject spoilerSpawn = previewCar.transform.Find("Spoiler_Spawn").gameObject;

        // Update spoiler Mesh
        spoilerSpawn.GetComponent<MeshFilter>().sharedMesh = spoilers[spoilerID].partObjectPrefab.GetComponent<MeshFilter>().sharedMesh;

        // Update part colour
        colours = spoilers[spoilerID].colour;
        UpdatePartColour(spoilerSpawn, colours.redChannel, colours.blueChannel, colours.greenChannel, colours.alphaChannel);

        GameObject wheelParentObject = previewCar.transform.Find("Wheels_Spawn").gameObject;
        for (int i = 0; i < wheelParentObject.transform.childCount; i++)
        {
            GameObject wheelChild = wheelParentObject.transform.GetChild(i).gameObject;
            wheelChild.GetComponent<MeshFilter>().sharedMesh = wheels[wheelID].partObjectPrefab.GetComponent<MeshFilter>().sharedMesh;

            // Update the colour
            colours = wheels[wheelID].colour;
            UpdatePartColour(wheelChild, colours.redChannel, colours.blueChannel, colours.greenChannel, colours.alphaChannel);
        }

        // Update Character
        GameObject characterSpawn = previewCar.transform.Find("Character_Spawn").gameObject;
        characterSpawn.GetComponent<MeshFilter>().sharedMesh = characters[characterID].partObjectPrefab.GetComponentInChildren<MeshFilter>().sharedMesh;

        // Update the visual display of stats
        UpdateSliders();
    }

    // Changes the material to match the colour picker settings
    void UpdatePartColour(GameObject obj, Color red, Color blue, Color green, Color alpha)
    {
        if (obj.GetComponent<Renderer>() == null)
            return;

        MaterialPropertyBlock props = new MaterialPropertyBlock();
        props.SetColor("_Color_Red", red);
        props.SetColor("_Color_Green", green);
        props.SetColor("_Color_Blue", blue);
        props.SetColor("_Color_Alpha", alpha);
        obj.GetComponent<Renderer>().SetPropertyBlock(props);
    }

    // Update is called once per frame
    void OnValidate()
    {
        if (GetComponent<Renderer>() == null)
            return;

        MaterialPropertyBlock props = new MaterialPropertyBlock();
        props.SetColor("_Color_Red", vehicleColour.redChannel);
        props.SetColor("_Color_Green", vehicleColour.greenChannel);
        props.SetColor("_Color_Blue", vehicleColour.blueChannel);
        props.SetColor("_Color_Alpha", vehicleColour.alphaChannel);
        GetComponent<Renderer>().SetPropertyBlock(props);

    }

    [Command]
    public void CmdRequestPartChange(int spoilerID, int wheelID, int bodyID, int characterID)
    {
        this.spoilerID = spoilerID;
        this.wheelID = wheelID;
        this.bodyID = bodyID;
        this.characterID = characterID;
    }

    [Command]
    public void CmdRequestColourChange(ColourStruct newColours)
    {
        vehicleColour = newColours;
    }

    public void IncreaseSpoilerID()
    {
        spoilerID++;
        if (spoilerID > spoilers.Count - 1)
            spoilerID = 0;

        spoilerButton.GetComponent<Image>().sprite = spoilers[spoilerID].partSprite;
        UpdatePreviewCar();
    }

    public void IncreaseWheelID()
    {
        wheelID++;
        if (wheelID > wheels.Count - 1)
            wheelID = 0;

        wheelButton.GetComponent<Image>().sprite = wheels[wheelID].partSprite;
        UpdatePreviewCar();
    }

    public void IncreaseBodyID()
    {
        bodyID++;
        if (bodyID > bodys.Count - 1)
            bodyID = 0;

        bodyButton.GetComponent<Image>().sprite = bodys[bodyID].partSprite;
        UpdatePreviewCar();
    }

    public void IncreaseCharacterID()
    {
        characterID++;
        if (characterID > characters.Count - 1)
            characterID = 0;

        characterButton.GetComponent<Image>().sprite = characters[characterID].partSprite;
        UpdatePreviewCar();
    }

    void UpdateSliders()
    {
        speedSlider.maxValue = 150;
        accelerationSlider.maxValue = 10;
        handlingSlider.maxValue = 3;

        speedSlider.value = spoilers[spoilerID].speed + wheels[wheelID].speed + bodys[bodyID].speed;
        accelerationSlider.value = spoilers[spoilerID].acceleration + wheels[wheelID].acceleration + bodys[bodyID].acceleration;
        handlingSlider.value = spoilers[spoilerID].handling + wheels[wheelID].handling + bodys[bodyID].handling;
    }
}
