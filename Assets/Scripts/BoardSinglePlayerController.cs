using Assets.Scripts.ScriptableObjects.Events;
using Assets.Scripts.ScriptableObjects.Variables;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public class BoardSinglePlayerController : MonoBehaviour
    {
        [SerializeField]
        private GameEvent _clearTiles;
        [SerializeField]
        private IntGameEvent _onTileClick;
        [SerializeField]
        private IntVariable _tilesAmountVariable;
        [SerializeField]
        private GameEvent _playerMovedEvent;
        [SerializeField]
        private GameEvent _playerWinEvent;
        [SerializeField]
        private GameEvent _initializeEvent;
        [SerializeField]
        private AudioClipGameEvent _onTileClickAudio;
        [SerializeField]
        private AudioClip _clickSound;
        [SerializeField]
        private IntIntGameEvent _createTileViewEvent;
        [SerializeField]
        private IntGameEvent _showTileEvent;
        [SerializeField]
        private IntGameEvent _hideTileEvent;
        [SerializeField]
        private IntGameEvent _setIsDoneTileEvent;

        private List<int> _randomNumberList = new();
        private List<int> _activeTiles = new();

        private List<TileController> _tiles = new();

        private void CreateBoard()
        {
            _randomNumberList.Clear();
            _activeTiles.Clear();

            for (int i = 0; i < _tilesAmountVariable.Value / 2; i++)
            {
                _randomNumberList.Add(i);
                _randomNumberList.Add(i);
            }

            Shuffle();

            for (int i = 0; i < _tilesAmountVariable.Value; i++)
            {
                TileController tileController = new TileController(i, _randomNumberList[i], string.Empty);
                _tiles.Add(tileController);
            }

            for (int i = 0; i < _randomNumberList.Count; i++)
            {
                _createTileViewEvent.Raise(i, _randomNumberList[i]);
            }
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

        private void OnEnable()
        {
            _initializeEvent.OnRaise += Initialize;
            _onTileClick.OnRaise += OnTileClicked;
        }

        private void OnDisable()
        {
            _initializeEvent.OnRaise -= Initialize;
            _onTileClick.OnRaise -= OnTileClicked;
        }

        private void Initialize()
        {
            _tiles.Clear();
            _activeTiles.Clear();
            _clearTiles.Raise();
            CreateBoard();
        }

        private void OnTileClicked(int tileId)
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

                _onTileClickAudio.Raise(_clickSound);
                _showTileEvent.Raise(tileId);

                _activeTiles.Add(tileId);

                if (_activeTiles.Count == 2)
                {
                    int firstIndex = _tiles.Find(x => x.TileId == _activeTiles[0]).TileIndex;
                    int secondIndex = _tiles.Find(x => x.TileId == _activeTiles[1]).TileIndex;

                    //Resolve(_activeTiles[0], _activeTiles[1]);
                    Resolve(firstIndex, secondIndex);
                }
            }
        }

        private async void Resolve(int firstIndex, int secondIndex)
        {
            _playerMovedEvent.Raise();
            
            if (firstIndex == secondIndex)
            {
                await Task.Delay(TimeSpan.FromSeconds(1f));

                List<TileController> tiles = _tiles.FindAll(x => x.TileIndex == firstIndex);
                foreach (TileController tile in tiles)
                {
                    tile.SetIsDone(true);
                    _setIsDoneTileEvent.Raise(tile.TileId);
                }

                if (_tiles.FindAll(x => !x.IsDone).Count == 0)
                {
                    _playerWinEvent.Raise();
                    return;
                }
            }
            else
            {
                await Task.Delay(TimeSpan.FromSeconds(1f));

                foreach (var activeTile in _activeTiles)
                {
                    _hideTileEvent.Raise(activeTile);
                }
            }

            _activeTiles.Clear();
        }

        private void Start()
        {
            _initializeEvent.Raise();
        }
    }
}