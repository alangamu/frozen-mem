﻿using Assets.Scripts.ScriptableObjects;
using Assets.Scripts.ScriptableObjects.Variables;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class LobbyManagerUI : MonoBehaviour
    {
        [SerializeField]
        private LobbyManager _lobbyManager;
        [SerializeField]
        private Transform _lobbyListTransform;
        [SerializeField]
        private GameObject _lobbyItemUIPrefab;
        [SerializeField]
        private Text _playerId;
        [SerializeField]
        private StringVariable _playerNameVariable;
        [SerializeField]
        private IntVariable _playerAvatarIndexVariable;
        [SerializeField]
        private StringVariable _keyStartGameVariable;
        [SerializeField]
        private Image _playerAvatar;
        [SerializeField]
        private AvatarModel _playerAvatarModel;
        [SerializeField]
        private Button _refreshLobbyListButton;

        private async void RefreshLobbyList()
        {
            _refreshLobbyListButton.enabled = false;
            ClearLobbyList();

            List<Lobby> lobbyList = await _lobbyManager.GetLobbyList();

            foreach (var item in lobbyList)
            {
                if (item.AvailableSlots > 0)
                {
                    GameObject lobbyItemUIObject = Instantiate(_lobbyItemUIPrefab, _lobbyListTransform);

                    if (lobbyItemUIObject.TryGetComponent(out LobbyItemUI lobbyItemUI))
                    {
                        lobbyItemUI.Initialize(item.Name, item.Id, item.Players.Count, item.MaxPlayers);
                    }
                }
            }

            await Task.Delay(TimeSpan.FromSeconds(1f));
            _refreshLobbyListButton.enabled = true;
        }

        private void OnEnable()
        {
            _refreshLobbyListButton.onClick.AddListener(RefreshLobbyList);
            _playerNameVariable.OnValueChanged += OnPlayerNameChanged;
            _playerAvatarIndexVariable.OnValueChanged += OnPlayerAvatarChanged;
        }

        private void OnDisable()
        {
            _refreshLobbyListButton.onClick.RemoveAllListeners();
            _playerNameVariable.OnValueChanged -= OnPlayerNameChanged;
            _playerAvatarIndexVariable.OnValueChanged -= OnPlayerAvatarChanged;
        }

        private void OnPlayerAvatarChanged(int newPlayerAvatarIndex)
        {
            _playerAvatar.sprite = _playerAvatarModel.Avatars[newPlayerAvatarIndex];
        }

        private void OnPlayerNameChanged(string newPlayerName)
        {
            _playerId.text = newPlayerName;
        }

        private void Start()
        {
            RefreshLobbyList();
            OnPlayerNameChanged(_playerNameVariable.Value);
            OnPlayerAvatarChanged(_playerAvatarIndexVariable.Value);
        }

        private void ClearLobbyList()
        {
            foreach (Transform item in _lobbyListTransform)
            {
                Destroy(item.gameObject);
            }
        }
    }
}