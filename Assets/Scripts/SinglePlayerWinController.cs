using Assets.Scripts.ScriptableObjects.Events;
using UnityEngine;

namespace Assets.Scripts
{
    public class SinglePlayerWinController : MonoBehaviour
    {
        [SerializeField]
        private GameEvent _playerWinEvent;
        [SerializeField]
        private GameEvent _restartSinglePlayerEvent;
        [SerializeField]
        private GameEvent _loadMainMenuEvent;
        [SerializeField]
        private Transform _winWindowTransform;
        [SerializeField]
        private GameEvent _restartMovesEvent;
        [SerializeField]
        private GameEvent _initializeEvent;
        [SerializeField]
        private StringGameEvent _sceneLoadEvent;
        [SerializeField]
        private string _sceneName;

        private void OnEnable()
        {
            _winWindowTransform.gameObject.SetActive(false);
            _playerWinEvent.OnRaise += PlayerWin;
            _restartSinglePlayerEvent.OnRaise += RestartSinglePlayer;
            _loadMainMenuEvent.OnRaise += LoadMainMenu;
        }

        private void OnDisable()
        {
            _playerWinEvent.OnRaise -= PlayerWin;
            _restartSinglePlayerEvent.OnRaise -= RestartSinglePlayer;
            _loadMainMenuEvent.OnRaise -= LoadMainMenu;
        }

        private void LoadMainMenu()
        {
            _sceneLoadEvent.Raise(_sceneName);
        }

        private void RestartSinglePlayer()
        {
            _winWindowTransform.gameObject.SetActive(false);
            _initializeEvent.Raise();
        }

        private void PlayerWin()
        {
            _winWindowTransform.gameObject.SetActive(true);
        }
    }
}