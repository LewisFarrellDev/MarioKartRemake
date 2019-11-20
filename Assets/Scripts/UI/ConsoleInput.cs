// Script Created By Lewis Farrell - S15118289 //

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Networking;

public class ConsoleInput : MonoBehaviour
{
    public bool isHorizontalNavigation;
    public List<GameObject> buttons = new List<GameObject>();

    private EventSystem eventSystem;
    private int maxIndex = 0;
    private int buttonIndex = 0;
    private int lastTick = 0;

    void Start()
    {
        NetworkIdentity networkIdentity = GetComponent<NetworkIdentity>();
        if (networkIdentity)
            if (!networkIdentity.isLocalPlayer)
                this.enabled = false;

        eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
        eventSystem.SetSelectedGameObject(buttons[buttonIndex]);
    }

    void OnEnable()
    {
        buttonIndex = 0;
        maxIndex = buttons.Count - 1;
    }

    void Update()
    {
        EventSystem system = GameObject.Find("EventSystem").GetComponent<EventSystem>();
        if (system != null)
            eventSystem = system;
        eventSystem.SetSelectedGameObject(buttons[buttonIndex]);

        switch (isHorizontalNavigation)
        {
            case true:
                if (Input.GetAxisRaw("MenuHorizontal") > 0.5f)
                    DecrementIndex();
                else if (Input.GetAxisRaw("MenuHorizontal") < -0.5f)
                    IncrementIndex();
                break;

            case false:
                if (Input.GetAxisRaw("MenuVertical") > 0.5f)
                    DecrementIndex();
                else if (Input.GetAxisRaw("MenuVertical") < -0.5f)
                    IncrementIndex();
                break;
        }



        if (Input.GetButtonDown("MenuSelect"))
        {
            eventSystem.currentSelectedGameObject.GetComponent<Button>().onClick.Invoke();
        }
    }

    void IncrementIndex()
    {
        if (lastTick + 200 > System.Environment.TickCount)
            return;

        buttonIndex++;
        if (buttonIndex > maxIndex)
            buttonIndex = 0;

        lastTick = System.Environment.TickCount;
    }

    void DecrementIndex()
    {
        if (lastTick + 200 > System.Environment.TickCount)
            return;

        buttonIndex--;
        if (buttonIndex < 0)
            buttonIndex = maxIndex;

        lastTick = System.Environment.TickCount;
    }
}
