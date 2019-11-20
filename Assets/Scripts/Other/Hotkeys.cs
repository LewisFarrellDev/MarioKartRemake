// Script Created By Lewis Farrell - S15118289 //

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Hotkeys : NetworkBehaviour
{
    float lastDeltaTime = 0;
    float heldCount;
    bool isHoldingLeaveGame;

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (!isHoldingLeaveGame)
            heldCount = 0;

        if (Input.GetJoystickNames().Length > 0)
            if (Input.GetJoystickNames()[0].IndexOf("360") >= 0)
                XboxLeaveGame();
            else
                PS4LeaveGame();
        else
            PCLeaveGame();

    }

    void PCLeaveGame()
    {
        if (Input.GetButton("PCLeaveGame"))
        {
            isHoldingLeaveGame = true;
            if (lastDeltaTime + 1000 < System.Environment.TickCount)
            {
                heldCount++;
                print("Held for: " + heldCount);
                lastDeltaTime = System.Environment.TickCount;
            }

            if (heldCount >= 3)
            {
                NetworkManager.singleton.GetComponent<CustomLobbyManager>().LeaveLobby();
            }
        }
        else
            isHoldingLeaveGame = false;
    }

    void XboxLeaveGame()
    {
        if (Input.GetButton("XboxLeaveGame"))
        {
            isHoldingLeaveGame = true;
            if (lastDeltaTime + 1000 < System.Environment.TickCount)
            {
                heldCount++;
                print("Held for: " + heldCount);
                lastDeltaTime = System.Environment.TickCount;
            }

            if (heldCount >= 3)
            {
                NetworkManager.singleton.GetComponent<CustomLobbyManager>().LeaveLobby();
            }
        }
        else
            isHoldingLeaveGame = false;
    }

    void PS4LeaveGame()
    {
        if (Input.GetButton("PS4LeaveGame"))
        {
            isHoldingLeaveGame = true;
            if (lastDeltaTime + 1000 < System.Environment.TickCount)
            {
                heldCount++;
                print("Held for: " + heldCount);
                lastDeltaTime = System.Environment.TickCount;
            }

            if (heldCount >= 3)
            {
                NetworkManager.singleton.GetComponent<CustomLobbyManager>().LeaveLobby();
            }
        }
        else
            isHoldingLeaveGame = false;
    }
}
