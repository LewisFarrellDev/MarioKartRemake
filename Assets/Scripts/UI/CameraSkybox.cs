using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSkybox : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        Camera CameraSkybox = GetComponent<Camera>();

        if (CameraSkybox != null)
        {
            CameraSkybox.backgroundColor = Camera.main.backgroundColor;
            Color color = Camera.main.backgroundColor;
            color.a = 1;
            CameraSkybox.backgroundColor = color;
            CameraSkybox.clearFlags = Camera.main.clearFlags;
        }



    }

    // Update is called once per frame
    void Update()
    {

    }
}
