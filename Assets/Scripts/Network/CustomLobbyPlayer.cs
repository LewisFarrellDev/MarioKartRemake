// Script Created By Lewis Farrell - S15118289 //

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CustomLobbyPlayer : NetworkLobbyPlayer
{
    private LobbyPlayerUI lobbyPlayerUI;
    private SetupVehicleCustomization setupVehicleCustomization;
    private bool isReady;

    void Awake()
    {
        // Assign variable
        lobbyPlayerUI = GetComponent<LobbyPlayerUI>();
    }

    // When the player readys up
    public override void OnClientReady(bool readyState)
    {
        base.OnClientReady(readyState);

        // Check if we have authority over this object (Only allow client of this object to change it, nobody else)
        if (!hasAuthority)
            return;

        setupVehicleCustomization = GetComponent<SetupVehicleCustomization>();


        // Change the colour on the server if the colour picker was found
        if (setupVehicleCustomization)
        {
            // Request Colour
            setupVehicleCustomization.CmdRequestColourChange(setupVehicleCustomization.vehicleColour);

            // Request Parts
            setupVehicleCustomization.CmdRequestPartChange(setupVehicleCustomization.spoilerID, setupVehicleCustomization.wheelID, setupVehicleCustomization.bodyID, setupVehicleCustomization.characterID);
        }
    }

    // Called when the Local Player has been set up
    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        // Since the local player has now been set up, we can set the UI up correctly for this player
        // E.G Enabling a ready button
        SetupLocalPlayer();

    }

    // When the client player enters the lobby
    public override void OnClientEnterLobby()
    {
        base.OnClientEnterLobby();

        // Parent this player to the correct UI Panel and Fix size
        transform.SetParent(NetworkManager.singleton.GetComponent<LobbyManagerUI>().lobbyPlayerPanel.transform);
        RectTransform rect = transform.GetComponent<RectTransform>();
        rect.localScale = new Vector3(1, 1, 1);
        rect.localPosition = new Vector3(0, 0, 0);
        
        // When any client joins the lobby, Set up their UI as if they were not the local player
        // E.G disabling the ready button etc
        // Set up the other player
        if (isLocalPlayer)
            SetupLocalPlayer();
        else
            SetupOtherPlayer();
    }

    // Specify how the UI looks for the local player
    void SetupLocalPlayer()
    {    
        if (lobbyPlayerUI)
            lobbyPlayerUI.lobbyUI.SetActive(true);
    }

    // Specify how the UI looks for the other players
    void SetupOtherPlayer()
    {
        if (lobbyPlayerUI)
            lobbyPlayerUI.lobbyUI.SetActive(false);

    }

    // Specifcy how the UI looks for the local player
    public void SetPlayerReady()
    {
        if (!isReady)
        {
            SendReadyToBeginMessage();
            isReady = true;
        }
    }

    // Leave the lobby
    public void LeaveLobby()
    {
        NetworkManager.singleton.GetComponent<CustomLobbyManager>().LeaveLobby();
        NetworkManager.singleton.GetComponent<LobbyManagerUI>().ShowMenu(NetworkManager.singleton.GetComponent<LobbyManagerUI>().mainMenu);
    }
}
