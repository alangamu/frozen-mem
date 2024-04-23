using Assets.Scripts.ScriptableObjects.Events;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class TileView : MonoBehaviour
    {
        public int TileId { get; private set; }

        [SerializeField]
        private IntGameEvent _onTileClick;
        [SerializeField]
        private Image _tileImage;
        [SerializeField]
        private Transform _tileIsDone;
        [SerializeField]
        private Button _tileButton;

        private Sprite _tileSprite;

        public void Initialize(Sprite tileSprite, int tileId)
        {
            _tileIsDone.gameObject.SetActive(false);
            _tileSprite = tileSprite;
            TileId = tileId;
        }

        public void ShowTile()
        {
            _tileImage.gameObject.SetActive(true);
            _tileImage.sprite = _tileSprite;
        }

        public void HideTile()
        {
            _tileImage.gameObject.SetActive(false);
            _tileImage.sprite = null;
        }

        public void SetIsDone()
        {
            _tileIsDone.gameObject.SetActive(true);
        }

        private void OnEnable()
        {
            _tileButton.onClick.AddListener( ()=> { _onTileClick.Raise(TileId); });
        }

        private void OnDisable()
        {
            _tileButton.onClick.RemoveAllListeners();
        }
    }
}