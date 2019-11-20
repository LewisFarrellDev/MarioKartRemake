// Script Created By Lewis Farrell - S15118289 //

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideRenderer : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        if (GetComponent<Renderer>() != null)
            GetComponent<Renderer>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
