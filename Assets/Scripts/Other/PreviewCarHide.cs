// Script Created By Lewis Farrell - S15118289 //

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewCarHide : MonoBehaviour
{
    private GameObject reference;

    public string referenceName;
    private Vector3 startPos;
    // Use this for initialization
    void Start()
    {
        startPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        reference = GameObject.Find(referenceName);
        if (reference == null)
        {
            transform.position = new Vector3(0, 1000, 0);
        }
        else
            transform.position = startPos;
    }
}
