using Assets.Scripts.ScriptableObjects.Events;
using Assets.Scripts.ScriptableObjects.Variables;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class BoardView : MonoBehaviour
    {
        [SerializeField]
        private Transform _tilesRootTransform;
        [SerializeField]
        private TileView _tilePrefab;
        [SerializeField]
        private TileSpritesVariable _tileSprites;
        [SerializeField]
        private AudioClipGameEvent _onTileClickAudio;
        [SerializeField]
        private AudioClip _clickSound;
        [SerializeField]
        private IntGameEvent _showTileEvent;
        [SerializeField]
        private IntGameEvent _hideTileEvent;
        [SerializeField]
        private IntGameEvent _setIsDoneTileEvent;
        [SerializeField]
        private GameEvent _clearTiles;
        [SerializeField]
        private IntIntGameEvent _createTileViewEvent;

        private List<TileView> _tiles = new();

        private void ShowTile(int tileId)
        {
            TileView tileView = _tiles.Find(x => x.TileId == tileId);
            tileView.ShowTile();
            _onTileClickAudio.Raise(_clickSound);
        }

        private void CreateTile(int tileId, int tileIndex)
        {
            TileView tileView = Instantiate(_tilePrefab, _tilesRootTransform);
            if (tileView != null)
            {
                tileView.Initialize(_tileSprites.Value[tileIndex], tileId);
                tileView.HideTile();
                tileView.name = $"Tile {tileId}";
                _tiles.Add(tileView);
            }
        }

        private void HideTile(int tileId)
        {
            TileView tileView = _tiles.Find(x => x.TileId == tileId);
            tileView.HideTile();
        }

        private void ClearTiles()
        {
            _tiles.Clear();
            foreach (Transform tileTransform in _tilesRootTransform)
            {
                Destroy(tileTransform.gameObject);
            }
        }

        private void SetIsDoneTile(int tileId)
        {
            TileView tileView = _tiles.Find(x => x.TileId == tileId);
            tileView.HideTile();
            tileView.SetIsDone();
        }

        private void OnEnable()
        {
            _showTileEvent.OnRaise += ShowTile;
            _clearTiles.OnRaise += ClearTiles;
            _hideTileEvent.OnRaise += HideTile;
            _createTileViewEvent.OnRaise += CreateTile;
            _setIsDoneTileEvent.OnRaise += SetIsDoneTile;
        }

        private void OnDisable()
        {
            _showTileEvent.OnRaise -= ShowTile;
            _clearTiles.OnRaise -= ClearTiles;
            _hideTileEvent.OnRaise -= HideTile;
            _createTileViewEvent.OnRaise -= CreateTile;
            _setIsDoneTileEvent.OnRaise -= SetIsDoneTile;
        }
    }
}