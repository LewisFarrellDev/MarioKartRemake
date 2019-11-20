// Script Created By Lewis Farrell - S15118289 //

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManagerUI : MonoBehaviour
{
    public GameObject lobbyPlayerPanel;

    public GameObject mainMenu;
    public GameObject multiplayerMenu;
    public GameObject lobbyMenu;
    public GameObject levelSelect;
    public GameObject options;

    public GameObject connectedPlayers;

    private List<GameObject> menus = new List<GameObject>();

    void Start()
    {
        menus.Add(mainMenu);
        menus.Add(multiplayerMenu);
        menus.Add(lobbyMenu);
        menus.Add(levelSelect);
        menus.Add(options);
    }

    void Update()
    {
        connectedPlayers.GetComponentInChildren<Text>().text = "Connected Players: " + lobbyMenu.transform.Find("Players").childCount;
    }

    public void ShowMenu(GameObject menu)
    {
        foreach (GameObject menuItem in menus)
        {
            if (menuItem != null)
                menuItem.SetActive(false);
        }

        menu.SetActive(true);

    }

    public void HideUI()
    {
        foreach (GameObject menuItem in menus)
        {
            menuItem.SetActive(false);
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

}
