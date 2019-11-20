// Script Created By Lewis Farrell - S15118289 //

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(LobbyManagerUI))]
[RequireComponent(typeof(CustomNetworkDiscovery))]
public class CustomLobbyManager : NetworkLobbyManager
{
    private LobbyManagerUI lobbyManagerUI;
    private NetworkDiscovery networkDiscovery;

    bool isLanGame;

    // You know what this does
    void Start()
    {
        lobbyManagerUI = GetComponent<LobbyManagerUI>();

        // Start up the network discovery
        networkDiscovery = GetComponent<NetworkDiscovery>();
    }

    // When the game scene is loaded for the player
    public override bool OnLobbyServerSceneLoadedForPlayer(GameObject lobbyPlayer, GameObject gamePlayer)
    {
        // Hide the UI when the scene changes
        lobbyManagerUI.HideUI();

        SetupNetworkVehicle setupNetworkVehicle = gamePlayer.GetComponent<SetupNetworkVehicle>();
        SetupVehicleCustomization setupVehicleCustomization = lobbyPlayer.GetComponent<SetupVehicleCustomization>();

        // Set the colour from the lobby player to the actual game player
        setupNetworkVehicle.colours = setupVehicleCustomization.vehicleColour;

        // Set the part id
        setupNetworkVehicle.wheelID = setupVehicleCustomization.wheelID;
        setupNetworkVehicle.spoilerID = setupVehicleCustomization.spoilerID;
        setupNetworkVehicle.bodyID = setupVehicleCustomization.bodyID;
        setupNetworkVehicle.characterID = setupVehicleCustomization.characterID;

        return base.OnLobbyServerSceneLoadedForPlayer(lobbyPlayer, gamePlayer);
    }

    // When the client scene changes
    public override void OnClientSceneChanged(NetworkConnection conn)
    {
        if (SceneManager.GetActiveScene().name != lobbyScene)
        {
            lobbyManagerUI.HideUI();
            base.OnClientSceneChanged(conn);

        }
    }

    // When all the players ready up
    public override void OnLobbyServerPlayersReady()
    {
        if (networkDiscovery.running)
            networkDiscovery.StopBroadcast();

        base.OnLobbyServerPlayersReady(); // Comment this out when using custom UI as it will change the level
        print("all players ready");

    }

    // When the client disconnects
    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);
        print("DISCONNECTED");

        lobbyManagerUI.ShowMenu(lobbyManagerUI.mainMenu);

        LeaveLobby();
    }

    // When the client starts
    public override void OnStartClient(NetworkClient lobbyClient)
    {
        base.OnStartClient(lobbyClient);
        // Change the UI
        lobbyManagerUI.ShowMenu(lobbyManagerUI.lobbyMenu);
    }

    // When the client stops
    public override void OnStopClient()
    {
        base.OnStopClient();
        lobbyManagerUI.ShowMenu(lobbyManagerUI.mainMenu);
    }

    // Used to set the level and launch
    public void ChangeServerLevel(string level)
    {
        playScene = level;

        HostGame();
    }

    // Used when returning to lobby
    public void ReturnToLobby()
    {
        SendReturnToLobby();
        lobbyManagerUI.ShowMenu(lobbyManagerUI.lobbyMenu);
        print("Changed");
    }

    // Use when you want to remove a player from the lobby
    public void LeaveLobby()
    {
        StopHost();
        StopClient();

        if (networkDiscovery.running)
            networkDiscovery.StopBroadcast();
    }

    // Starts a lan game with the client also being a server
    // This is what we want in most cases
    public void Lan_Host()
    {
       
        if (networkDiscovery.running)
            networkDiscovery.StopBroadcast();

        if (!networkDiscovery.Initialize())
        {
            print("Failed to start network discovery");
            return;
        }

        // Check if the discovery is running is client or server mode
        // if it is, stop this and start again
        if (!networkDiscovery.isServer)
        {
            networkDiscovery.StartAsServer();
            StartHost();
        }

    }

    // Allows a client to connect to the lan server if it exists
    public void Lan_Connect()
    {
        
        if (networkDiscovery.running)
            networkDiscovery.StopBroadcast();

        if (!networkDiscovery.Initialize())
        {
            print("Failed to start network discovery");
            return;
        }

        // Check if the discovery is running is client or server mode
        // if it is, stop this and start again
        if (!networkDiscovery.isClient)
        {
            networkDiscovery.StartAsClient();
        }
    }

    // Create a dedicated server
    // This creates a server but does not allow the person who create it to also play as a client
    public void Lan_StartServer()
    {
        StartServer();
    }

    // Stop hosting a server
    public void Lan_StopHosting()
    {
        StopHost();
    }

    // Starts the matchmaking service
    // This needs to be done before the matchmaker can be used
    public void Matchmaker_Start()
    {
        StartMatchMaker();
    }

    // Create a new matchmaking server
    public void Matchmaker_Create()
    {
        matchMaker.CreateMatch(matchName, matchSize, true, "", "", "", 0, 0, OnMatchCreate);
    }

    // Lists the current matchmaking servers available
    public void Matchmaker_ListServers()
    {
        matchMaker.ListMatches(0, 20, "", true, 0, 0, OnMatchList);
    }

    // Stops the matchmaking service
    public void Matchermaker_Stop()
    {
        StopMatchMaker();
    }

    // Join the match
    public void Matchmaker_Join()
    {
        StartCoroutine(JoinMatchDelay());
    }

    // Delay join to make sure the matches are found
    public IEnumerator JoinMatchDelay()
    {
        yield return new WaitForSeconds(1);
        if (matches.Count != 0)
        {
            matchMaker.JoinMatch(matches[0].networkId, "", "", "", 0, 0, OnMatchJoined);
        }
    }

    // Create correct type of game based on menu
    void HostGame()
    {
        if (isLanGame)
        {
            Lan_Host();
        }
        else
        {
            Matchmaker_Start();
            Matchmaker_Create();
        }
    }

    public void SetIsLanGame(bool isLanGame)
    {
        this.isLanGame = isLanGame;
    }
}
