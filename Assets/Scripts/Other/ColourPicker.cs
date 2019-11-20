// Script Created By Lewis Farrell - S15118289 //

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public struct ColourStruct
{
    public Color redChannel;
    public Color greenChannel;
    public Color blueChannel;
    public Color alphaChannel;

    public ColourStruct(Color red, Color green, Color blue, Color alpha)
    {
        redChannel = red;
        greenChannel = green;
        blueChannel = blue;
        alphaChannel = alpha;
    }
}

[ExecuteInEditMode]
public class ColourPicker : MonoBehaviour
{
    public ColourStruct colours;

    // Start is called before the first frame update
    void Start()
    {
        OnValidate();
    }

    // Update is called once per frame
    void OnValidate()
    {
        if (GetComponent<Renderer>() == null)
            return;

        MaterialPropertyBlock props = new MaterialPropertyBlock();
        props.SetColor("_Color_Red", colours.redChannel);
        props.SetColor("_Color_Green", colours.greenChannel);
        props.SetColor("_Color_Blue", colours.blueChannel);
        props.SetColor("_Color_Alpha", colours.alphaChannel);
        GetComponent<Renderer>().SetPropertyBlock(props);

    }
}
