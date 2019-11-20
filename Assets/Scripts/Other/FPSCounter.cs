// Script Created By Lewis Farrell - S15118289 //

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FPSCounter : MonoBehaviour
{
    float fps;
    Text fpsText;
    float lastDeltaTime = 0;

    // Use this for initialization
    void Start()
    {
        fpsText = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if (lastDeltaTime + 1000 < System.Environment.TickCount)
        {
            fps = Mathf.Round(1.0f / Time.deltaTime);
            fpsText.text = "FPS: " + fps.ToString();
            lastDeltaTime = System.Environment.TickCount;
        }
    }
}
