// Script Created By Lewis Farrell - S15118289 //

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public int checkpointID;

    void OnTriggerEnter(Collider other)
    {
        CheckpointManager checkpointManager = other.GetComponent<CheckpointManager>();

        if (checkpointManager == null)
            return;

        if (checkpointManager)
            checkpointManager.SetCurrentCheckpoint(checkpointID);

        if (checkpointManager.IsPlayerFinished())
            other.GetComponent<CheckpointManager>().OnEndRace(RaceManager.instance.isTimeAttack);
    }
}
