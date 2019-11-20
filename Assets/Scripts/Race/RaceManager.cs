// Script Created By Lewis Farrell - S15118289 //

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class RaceManager : NetworkBehaviour
{
    public delegate void StartRaceDelegate();
    public static event StartRaceDelegate EventStartRace;

    public delegate void EndRaceDelegate(bool isTimeAttack);
    public static event EndRaceDelegate EventEndRace;

    public static RaceManager instance = null;

    private Dictionary<uint, int> playerPositions = new Dictionary<uint, int>();
    private List<uint> finishedPlayers = new List<uint>();

    public int numLaps;
    public int numCheckpointsPerLap;
    public int maxCountdown = 6;
    public bool isTimeAttack;
    [SyncVar]
    public int timeAttackTime;
    public GameObject countdownUI;
    private Text countdownText;

    [SyncVar]
    private int totalConnections = 0;
   
    [HideInInspector]
    [SyncVar]
    public int currentCountdown;
    bool isRaceStarted;

    // End Race Stuff
    int endRaceTime = 10;
    bool isRaceEnded;

    AudioSource audioSource;
    public List<AudioClip> audioClips = new List<AudioClip>();

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        EventStartRace = null;
        EventEndRace = null;

        countdownUI.SetActive(false);
        countdownText = countdownUI.GetComponentInChildren<Text>();
        currentCountdown = maxCountdown;
        StartCoroutine(SlowUpdate());
    }

    IEnumerator SlowUpdate()
    {
        yield return new WaitForSeconds(3);
        while (true)
        {
            yield return new WaitForSeconds(1);
            if (hasAuthority)
                CmdCountdown();
        }
    }

    [Command]
    void CmdCountdown()
    {
        if (hasAuthority)
            CmdGetConnectionCount();

        // countdown stuff
        if (currentCountdown > 0)
        {
            audioSource.clip = audioClips[0];
            audioSource.time = 0.5f;
            audioSource.Play();

            currentCountdown--;
            RpcUpdateText(currentCountdown.ToString(), true);

        }
        else
        {
            RpcUpdateText("", false);
        }

        if (currentCountdown == 0 && !isRaceStarted)
        {
            audioSource.clip = audioClips[1];
            audioSource.Play();

            RpcUpdateText("GO!", true);
            RpcStartRace();
            isRaceStarted = true;
            if (isTimeAttack)
                StartCoroutine(TimeAttack());
        }

    }

    [ClientRpc]
    void RpcStartRace()
    {
        if (EventStartRace != null) 
            EventStartRace();
    }

    [ClientRpc]
    void RpcEndRace()
    {
        if (EventEndRace != null)
            EventEndRace(isTimeAttack);
    }

    [ClientRpc]
    void RpcUpdateText(string text, bool isVisible)
    {
        countdownUI.SetActive(isVisible);
        countdownText.text = text;
    }

    [Command]
    void CmdGetConnectionCount()
    {
        int count = 0;
        foreach(NetworkConnection con in NetworkServer.connections)
        {
            if (con != null)
                count++;
        }

        totalConnections = count;
    }

    public int GetTotalConnections()
    {
        return totalConnections;
    }

    public void AddFinishedPlayer(uint playerID)
    {
        finishedPlayers.Add(playerID);

        if (finishedPlayers.Count >= totalConnections)
        {
            StartCoroutine(EndRace());
        }

        if (finishedPlayers.Count > 0 && !isRaceEnded)
            StartCoroutine(EndRaceCountdown());
    }

    public int GetPlayerPosition(uint playerID)
    {
        int totalCheckpoints = playerPositions[playerID];
        int highCount = 0;
        foreach (KeyValuePair<uint, int> pair in playerPositions)
        {
            if (totalCheckpoints > pair.Value)
                highCount++;
        }

        int position = playerPositions.Count - highCount;
        return position;
    }

    public bool IsPlayerFinished(uint playerID)
    {
        if (finishedPlayers.Contains(playerID))
            return true;

        return false;
    }

    public void UpdatePlayerPosition(uint playerID, int totalCheckpoints)
    {
        if (!playerPositions.ContainsKey(playerID))
            playerPositions.Add(playerID, totalCheckpoints);

        playerPositions[playerID] = totalCheckpoints;
    }

    IEnumerator EndRaceCountdown()
    {
        while (endRaceTime != 0)
        {
            yield return new WaitForSeconds(1);
            endRaceTime--;
        }

        if (!isRaceEnded && !isTimeAttack)
            StartCoroutine(EndRace());
    }
   
    IEnumerator EndRace()
    {
        if (EventEndRace != null)
            RpcEndRace();

        isRaceEnded = true;
        print("RACE FINISHED - Stopping server in 5 seconds");
        yield return new WaitForSeconds(5);
        print("RACE FINISHED - Server Stopped");
        NetworkManager.singleton.GetComponent<CustomLobbyManager>().StopHost();
    }

    IEnumerator TimeAttack()
    {
        if (isServer)
        {
            while (timeAttackTime > 0)
            {
                timeAttackTime--;
                yield return new WaitForSeconds(1);
            }

            if (!isRaceEnded)
                StartCoroutine(EndRace());
        }
    }
}
