using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Car Part", menuName = "New Car part")]
public class CarPart : ScriptableObject
{
    public string partName = "Default Name";
    public Sprite partSprite;
    [SerializeField]
    public GameObject partObjectPrefab;
    [SerializeField]
    public ColourStruct colour;
    [SerializeField]
    public float speed;
    [SerializeField]
    public float acceleration;
    [SerializeField]
    public float handling;

    public float getSpeed()
    {
        return speed;
    }
    public float getAceleration()
    {
        return acceleration;
        
    }
    public float getHandling()
    {
        return handling;
    }
}

