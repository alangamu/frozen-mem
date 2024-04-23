using Assets.Scripts.ScriptableObjects.Events;
using Assets.Scripts.ScriptableObjects.Variables;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Services.Authentication;
using UnityEngine;

namespace Assets.Scripts
{
    public class BoardManager : NetworkBehaviour
    {
        [SerializeField]
        private IntGameEvent _onTileClick;
        [SerializeField]
        private IntVariable _tilesAmountVariable;
        [SerializeField]
        private GameEvent _clearTiles;
        [SerializeField]
        private IntIntGameEvent _createTileViewEvent;
        [SerializeField]
        private IntGameEvent _showTileEvent;
        [SerializeField]
        private IntGameEvent _hideTileEvent;
        [SerializeField]
        private IntGameEvent _setIsDoneTileEvent;

        [SerializeField]
        private StringVariable _activePlayerId;
        [SerializeField]
        private GameEvent _gameStartEvent;
        [SerializeField]
        private GameEvent _initializeEvent;
        [SerializeField]
        private GameEvent _endTurnEvent;
        [SerializeField]
        private StringGameEvent _playerScored;
        [SerializeField]
        private GameEvent _gameOverEvent;
        [SerializeField]
        private GameEvent _stopCountdown;
        [SerializeField]
        private GameEvent _restartTurnEvent;

        private List<int> _randomNumberList = new();
        private List<int> _activeTiles = new();
        private string _playerId;

        private List<TileController> _tiles = new();

        private void OnEnable()
        {
            _onTileClick.OnRaise += OnTileClicked;

            _endTurnEvent.OnRaise += EndTurn;
            _initializeEvent.OnRaise += Initialize;
            _playerId = AuthenticationService.Instance.PlayerId;
        }

        private void OnDisable()
        {
            _onTileClick.OnRaise -= OnTileClicked;

            _endTurnEvent.OnRaise -= EndTurn;
            _initializeEvent.OnRaise -= Initialize;
        }

        private void EndTurn()
        {
            foreach (var activeTile in _activeTiles)
            {
                HideContentRpc(activeTile);
            }

            _activeTiles.Clear();
        }

        private void OnTileClicked(int tileId)
        {
            TileController tileController = _tiles.Find(x => x.TileId == tileId);
            if (tileController != null)
            {
                if (_activePlayerId.Value.Equals(tileController.PlayerId) && !tileController.IsDone)
                {
                    //var index = Array.FindIndex(_tileControllers, x => x == tileController);
                    PressTileRpc(tileController.TileId, tileController.PlayerId);
                }
            }
        }

        [Rpc(SendTo.Server)]
        private void PressTileRpc(int tileId, string playerId)
        {
            if (_activeTiles.Count < 2)
            {
                if (_activeTiles.Count == 1)
                {
                    //double click or click the same tile
                    if (_activeTiles.Contains(tileId))
                    {
                        return;
                    }
                }

                ShowTileRpc(tileId);
                _activeTiles.Add(tileId);

                if (_activeTiles.Count == 2)
                {
                    CancelCountdownRpc();

                    int firstIndex = _tiles.Find(x => x.TileId == _activeTiles[0]).TileIndex;
                    int secondIndex = _tiles.Find(x => x.TileId == _activeTiles[1]).TileIndex;

                    Resolve(playerId, firstIndex, secondIndex);
                }
            }
        }

        [Rpc(SendTo.Everyone)]
        private void ShowTileRpc(int tileId)
        {
            _showTileEvent.Raise(tileId);
        }

        private void Initialize()
        {
            _clearTiles.Raise();
            _activeTiles.Clear();
            _tiles.Clear();

            if (NetworkManager.Singleton.IsServer)
            {
                CreateBoard();
            }
        }

        public void CreateBoard()
        {
            _randomNumberList.Clear();

            for (int i = 0; i < _tilesAmountVariable.Value / 2; i++)
            {
                _randomNumberList.Add(i);
                _randomNumberList.Add(i);
            }

            Shuffle();

            for (int i = 0; i < _tilesAmountVariable.Value; i++)
            {
                CreateTileControllerRpc(i, _randomNumberList[i] );
            }

            for (int i = 0; i < _randomNumberList.Count; i++)
            {
                CreateTileViewRpc(i, _randomNumberList[i]);
            }

            StartGameRpc();
            
            //TODO: debug
            PrintPairs();
        }

        [Rpc(SendTo.Everyone)]
        private void CreateTileControllerRpc(int tileId, int tileIndex)
        {
            TileController tileController = new TileController(tileId, tileIndex, _playerId);
            _tiles.Add(tileController);
        }

        [Rpc(SendTo.Everyone)]
        private void CreateTileViewRpc(int tileId, int tileIndex)
        {
            _createTileViewEvent.Raise(tileId, tileIndex);
        }

        [Rpc(SendTo.Everyone)]
        private void StartGameRpc()
        {
            StartGame();
        }

        private async void StartGame()
        {
            await Task.Delay(TimeSpan.FromSeconds(2f));

            //HideContentClientRpc();
            _gameStartEvent.Raise();
            //StartGameClientRpc();
        }

        private void Shuffle()
        {
            int n = _randomNumberList.Count;
            System.Random rng = new System.Random();

            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                int value = _randomNumberList[k];
                _randomNumberList[k] = _randomNumberList[n];
                _randomNumberList[n] = value;
            }
        }

        private async void Resolve(string playerId, int firstIndex, int secondIndex)
        {
            if (firstIndex == secondIndex)
            {
                PlayerScoredRpc(playerId);

                await Task.Delay(1000);
                LockTilesRpc(firstIndex);

                if (_tiles.FindAll(x => !x.IsDone).Count == 0)
                {
                    EndGameClientRpc();
                    return;
                }

                RestartTurnRpc();
            }
            else
            {
                _activePlayerId.SetValue(string.Empty);
                await Task.Delay(1000);
                EndTurnRpc();

                foreach (var activeTile in _activeTiles)
                {
                    HideContentRpc(activeTile);
                }
            }

            _activeTiles.Clear();
        }

        [Rpc(SendTo.Everyone)]
        private void PlayerScoredRpc(string playerId)
        {
            _playerScored.Raise(playerId);
        }

        [Rpc(SendTo.Everyone)]
        private void CancelCountdownRpc()
        {
            _stopCountdown.Raise();
        }

        [Rpc(SendTo.Everyone)]
        private void RestartTurnRpc()
        {
            _restartTurnEvent.Raise();
        }

        [Rpc(SendTo.Everyone)]
        private void EndTurnRpc()
        {
            _endTurnEvent.Raise();
        }

        //[Rpc(SendTo.Everyone)]
        //private void StartGameClientRpc()
        //{
        //    _gameStartEvent.Raise();
        //}

        [Rpc(SendTo.Everyone)]
        private void LockTilesRpc(int index)
        {
            List<TileController> tiles = _tiles.FindAll(x => x.TileIndex == index) ;
            foreach (TileController tile in tiles)
            {
                tile.SetIsDone(true);
                _setIsDoneTileEvent.Raise(tile.TileId);
            }
        }

        [Rpc(SendTo.Everyone)]
        private void HideContentRpc(int tileId)
        {
            _hideTileEvent.Raise(tileId);
        }

        [ClientRpc]
        private void EndGameClientRpc()
        {
            _gameOverEvent.Raise();
        }

        //Degub only
        private void PrintPairs()
        {
            for (int i = 0; i < _tilesAmountVariable.Value / 2; i++)
            {
                List<TileController> debugTiles = _tiles.FindAll(x => x.TileIndex == i);

                foreach (TileController tile in debugTiles)
                {
                    Debug.Log($"tile index {i} -> {tile.TileId}");
                }
            }
        }
    }
}