// Script Created By Lewis Farrell - S15118289 //

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class BasePowerup : ScriptableObject
{
    public Sprite powerupSprite;
    public int numUses = 1;
    public bool requiresNetwork = false;

    abstract public void ActivatePowerup(GameObject playerRef);
}
