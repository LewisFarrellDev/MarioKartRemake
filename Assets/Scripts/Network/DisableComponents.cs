using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DisableComponents : NetworkBehaviour
{
    public GameObject minimapUI;
    public Camera minimapCam;

    void Start()
    {
        if (!hasAuthority)
        {
            minimapCam.enabled = false;
            minimapUI.SetActive(false);
        }
    }
}
