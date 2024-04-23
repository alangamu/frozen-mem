using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class WinScreenView : MonoBehaviour
    {
        public event Action OnPlayerPressedRematch;
        public event Action OnPlayerPressedRestart;

        public PlayerWinView[] PlayerWinViews => _playerWinViews;

        [SerializeField]
        private Button _remachtButton;
        [SerializeField]
        private Button _restartButton;
        [SerializeField]
        private PlayerWinView[] _playerWinViews;

        private void OnEnable()
        {
            _remachtButton.onClick.AddListener(() => { 
                OnPlayerPressedRematch?.Invoke(); 
            });

            _restartButton.onClick.AddListener(() => {
                OnPlayerPressedRestart?.Invoke();
            });
        }

        private void OnDisable()
        {
            _remachtButton.onClick.RemoveAllListeners();
            _restartButton.onClick.RemoveAllListeners();
        }
    }
}