// Script Created By Lewis Farrell - S15118289 //

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpeedPowerup", menuName = "Powerups/SpeedPowerup", order = 1)]
public class SpeedPowerup : BasePowerup
{
    public float SpeedIncrease = 150;
    public float duration = 5;

    public override void ActivatePowerup(GameObject playerRef)
    {
        VehicleController_V2 vehicle = playerRef.GetComponent<VehicleController_V2>();
        vehicle.StartCoroutine(vehicle.BoostSpeed(SpeedIncrease, duration));
    }
}
