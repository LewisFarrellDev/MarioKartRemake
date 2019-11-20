// Script Created By Lewis Farrell - S15118289 //

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class CheckpointManager : NetworkBehaviour
{
    public GameObject positionUI;
    public GameObject EndRaceUI;
    public GameObject lapObjectUI;
    public GameObject timeAttackUI;

    private int currentLap = 1;
    private int nextCheckpoint = 0;
    private int totalCheckpointsHit;
    private bool isFinished;

    private int position;
    private string[] positionSuffixes = new string[] { "1st", "2nd", "3rd", "4th", "5th", "6th", "7th", "8th" };

    private NetworkIdentity networkIdentity;

    public void Start()
    {
        networkIdentity = GetComponent<NetworkIdentity>();
        RaceManager.instance.UpdatePlayerPosition(networkIdentity.netId.Value, 0);

        RaceManager.EventEndRace += OnEndRace;

        timeAttackUI.SetActive(false);

        // Disable UI of other players
        if (!hasAuthority)
        {
            positionUI.SetActive(false);
            lapObjectUI.SetActive(false);
        }
        else
        {
            if (RaceManager.instance.isTimeAttack)
                timeAttackUI.SetActive(true);
        }
    }

    void Update()
    {
        if (hasAuthority)
        {
            // get position from race manager and set the text to visualy display it
            position = RaceManager.instance.GetPlayerPosition(networkIdentity.netId.Value);
            Text text = positionUI.GetComponentInChildren<Text>();
            text.text = "Position \n" + position.ToString() + " / " + RaceManager.instance.GetTotalConnections();

            // Do the same as above but for lap count
            int maxLaps = RaceManager.instance.numLaps;
            lapObjectUI.GetComponentInChildren<Text>().text = "Lap \n" + currentLap.ToString() + " / " + maxLaps;

            // Update timer 
            timeAttackUI.GetComponentInChildren<Text>().text = "Time \n" + Mathf.FloorToInt(RaceManager.instance.timeAttackTime / 60).ToString("00") + ":" + Mathf.FloorToInt(RaceManager.instance.timeAttackTime % 60).ToString("00");
        }
    }

    public void SetCurrentCheckpoint(int checkpointID)
    {
        if (checkpointID != nextCheckpoint)
            return;

        nextCheckpoint++;
        totalCheckpointsHit++;
        RaceManager.instance.UpdatePlayerPosition(networkIdentity.netId.Value, totalCheckpointsHit);

        if (nextCheckpoint > RaceManager.instance.numCheckpointsPerLap)
        {
            nextCheckpoint = 0;
            currentLap++;
        }

        if (currentLap > RaceManager.instance.numLaps)
        {
            RaceManager.instance.AddFinishedPlayer(networkIdentity.netId.Value);
            isFinished = true;
            currentLap = RaceManager.instance.numLaps;
        }
    }

    public bool IsPlayerFinished()
    {
        return isFinished;
    }

    public void OnEndRace(bool isTimeAttack)
    {
        if (hasAuthority)
        {
            if (!isTimeAttack)
            {
                GetComponent<VehicleController_V2>().SetControllable(false);
                EndRaceUI.SetActive(true);
                Text endRaceText = EndRaceUI.GetComponentInChildren<Text>();
                endRaceText.text = "You finished: " + positionSuffixes[position - 1];
            }
            else
            {
                GetComponent<VehicleController_V2>().SetControllable(false);
                EndRaceUI.SetActive(true);
                Text endRaceText = EndRaceUI.GetComponentInChildren<Text>();

                if (RaceManager.instance.IsPlayerFinished(networkIdentity.netId.Value))
                    endRaceText.text = "Record Beat with " + RaceManager.instance.timeAttackTime + " Second(s) remaining!";
                else
                    endRaceText.text = "Race Failed";
            }
        }
    }
}
