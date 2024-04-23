using Assets.Scripts.ScriptableObjects.Events;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.ScriptableObjects
{
    public class SceneChanger : MonoBehaviour
    {
        [SerializeField]
        private StringGameEvent _sceneLoadEvent;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        private void OnEnable()
        {
            _sceneLoadEvent.OnRaise += LoadScene;
        }

        private void OnDisable()
        {
            _sceneLoadEvent.OnRaise -= LoadScene;
        }

        private void LoadScene(string sceneName)
        {
            //TODO: make transition with color and duration
            SceneManager.LoadScene(sceneName);
        }
    }
}