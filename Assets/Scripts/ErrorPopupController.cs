using Assets.Scripts.ScriptableObjects.Events;
using Assets.Scripts.ScriptableObjects.Variables;
using Unity.Netcode;
using UnityEngine;

namespace Assets.Scripts
{
    public class ErrorPopupController : NetworkBehaviour
    {
        [SerializeField]
        private ErrorPopupView _errorPopupView;
        [SerializeField]
        private StringVariable _errorMessageVariable;
        [SerializeField]
        private StringGameEvent _sceneLoadEvent;
        [SerializeField]
        private string _sceneName;

        private void OnEnable()
        {
            _errorPopupView.gameObject.SetActive(false);
            _errorMessageVariable.OnValueChanged += OnErrorOccurred;
            _errorPopupView.OnPressedOk += OnPressedOk;
        }

        private void OnDisable()
        {
            _errorMessageVariable.OnValueChanged -= OnErrorOccurred;
            _errorPopupView.OnPressedOk -= OnPressedOk;
        }

        private void OnPressedOk()
        {
            _errorPopupView.gameObject.SetActive(false);
            NetworkManager.Singleton.Shutdown();
            _sceneLoadEvent.Raise(_sceneName);
        }

        private void OnErrorOccurred(string errorMessage)
        {
            _errorPopupView.gameObject.SetActive(true);
            _errorPopupView.SetErrorText(errorMessage);
        }
    }
}