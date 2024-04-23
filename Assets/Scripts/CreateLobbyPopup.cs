using Assets.Scripts.ScriptableObjects.Events;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class CreateLobbyPopup : MonoBehaviour
    {
        [SerializeField]
        private Text _lobbyNameText;
        [SerializeField]
        private LobbyManager _lobbyManager;
        [SerializeField]
        private StringGameEvent _sceneLoadEvent;
        [SerializeField]
        private string _sceneName;

        public async void CreateLobby()
        {
            await _lobbyManager.CreateLobby(_lobbyNameText.text);

            _sceneLoadEvent.Raise(_sceneName);
        }
    }
}