using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public enum eLobbyPanel
{
    Collection,
    Lobby,
    Store
}

public class LobbyUI : MonoBehaviour
{
    [SerializeField] Toggle collectionTg;
    [SerializeField] Toggle lobbyTg;
    [SerializeField] Toggle storeTg;

    Dictionary<eLobbyPanel, ALobbyPanel> lobbyPanels;

    eLobbyPanel currType;

    public void Init()
    {
        lobbyPanels = FindObjectsOfType<ALobbyPanel>(true).ToDictionary(x => x.type);
        foreach (var panel in lobbyPanels.Values)
        {
            panel.Init();
        }

        collectionTg.onValueChanged.AddListener((isOn) =>
        {
            if (isOn)
            {
                currType = eLobbyPanel.Collection;
                lobbyPanels[currType].Enter();
            }
            else
            {
                lobbyPanels[eLobbyPanel.Collection].Exit();
            }
        });

        lobbyTg.onValueChanged.AddListener((isOn) =>
        {
            if (isOn)
            {
                currType = eLobbyPanel.Lobby;
                lobbyPanels[currType].Enter();
            }
            else
            {
                lobbyPanels[eLobbyPanel.Lobby].Exit();
            }
        });

        storeTg.onValueChanged.AddListener((isOn) =>
        {
            if (isOn)
            {
                currType = eLobbyPanel.Store;
                lobbyPanels[currType].Enter();
            }
            else
            {
                lobbyPanels[eLobbyPanel.Store].Exit();
            }
        });
    }

    public void Enter()
    {
        lobbyTg.SetIsOnWithoutNotify(true);
        lobbyTg.Select();

        currType = eLobbyPanel.Lobby;
        lobbyPanels[currType].Enter();

        gameObject.SetActive(true);
    }

    public void Exit()
    {
        foreach (var panel in lobbyPanels.Values)
        {
            panel.Exit();
        }

        gameObject.SetActive(false);
    }

    public void SetUpdateCurrentPanel()
    {
        lobbyPanels[currType].UpdateUI();
    }
}
