﻿using Assets.Scripts.ScriptableObjects;
using Assets.Scripts.ScriptableObjects.Events;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class GameManager : NetworkBehaviour
    {
        [SerializeField]
        private GameEvent _initializeEvent;
        [SerializeField] 
        private LobbyManager _lobbyManager;
        [SerializeField]
        private WinScreenView _winScreenView;
        [SerializeField]
        private GameEvent _gameOverEvent;
        [SerializeField]
        private ScoreModel _scoreManager;
        [SerializeField]
        private PlayersModel _playersManager;
        [SerializeField]
        private GameEvent _rematchEvent;
        [SerializeField]
        private Transform _loadingWidgetTransform;

        [SerializeField]
        private Button _restartButton;

        private void OnEnable()
        {
            _gameOverEvent.OnRaise += OnGameOver;
            _rematchEvent.OnRaise += OnRematch;
        }

        private void OnDisable()
        {
            _gameOverEvent.OnRaise -= OnGameOver;
            _rematchEvent.OnRaise -= OnRematch;
        }

        private void OnRematch()
        {
            if (NetworkManager.Singleton.IsServer)
            {
                InitializeRpc();
            }
        }

        private void OnGameOver()
        {
            _winScreenView.gameObject.SetActive(true);

            if (NetworkManager.Singleton.IsHost)
            {
                Dictionary<string, int> score = _scoreManager.Score;
                int index = 1;
                while (_scoreManager.Score.Count > 0)
                {
                    string idPlayerHighScore = _scoreManager.GetIdPlayerHighScore(score);
                    ShowWinPanelClientRpc(_playersManager.Players[idPlayerHighScore].PlayerName, score[idPlayerHighScore], index++, idPlayerHighScore);
                    score.Remove(idPlayerHighScore);
                }
            }
        }

        private async void Start()
        {
            _winScreenView.gameObject.SetActive(false);
            _loadingWidgetTransform.gameObject.SetActive(true);
            _lobbyManager.StopHeartbeat();
            //TODO: listen to loaded clients connected or something like that
            if (NetworkManager.Singleton.IsServer)
            {
                await _lobbyManager.DeleteLobby(_lobbyManager.JoinedLobbyId);
                await Task.Delay(TimeSpan.FromSeconds(3f));
                InitializeRpc();
            }
        }

        [Rpc(SendTo.Everyone)]
        private void InitializeRpc()
        {
            _loadingWidgetTransform.gameObject.SetActive(false);
            _winScreenView.gameObject.SetActive(false);
            _initializeEvent.Raise();
        }

        [ClientRpc]
        private void ShowWinPanelClientRpc(string playerName, int playerScore, int index, string playerId)
        {
            _winScreenView.PlayerWinViews[index].gameObject.SetActive(true);
            _winScreenView.PlayerWinViews[index].Initialize(playerName, playerScore, index, playerId);
        }

        private void OnApplicationQuit()
        {
            NetworkManager.Singleton.Shutdown();
        }
    }
}