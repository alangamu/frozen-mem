using Assets.Scripts.ScriptableObjects.Events;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;


namespace Assets.Scripts.ScriptableObjects
{
    [CreateAssetMenu]
    public class PopupManager : ScriptableObject
    {
        [SerializeField]
        private GameObjectEvent _createPopupEvent;
        [SerializeField]
        private Color _backgroundColor = new Color(10.0f / 255.0f, 10.0f / 255.0f, 10.0f / 255.0f, 0.6f);

        private GameObject _background;
        private GameObject _popup;

        public async void Close()
        {
            var animator = _popup.GetComponent<Animator>();
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Open"))
                animator.Play("Close");

            RemoveBackground();
            await RunPopupDestroy();
        }

        private async Task RunPopupDestroy()
        {
            await Task.Delay(TimeSpan.FromSeconds(0.5f));
            Destroy(_background);
            Destroy(_popup);
        }

        private void RemoveBackground()
        {
            var image = _background.GetComponent<Image>();
            if (image != null)
                image.CrossFadeAlpha(0.0f, 0.2f, false);
        }

        private void OnEnable()
        {
            _createPopupEvent.OnRaise += CreatePopup;
        }

        private void OnDisable()
        {
            _createPopupEvent.OnRaise -= CreatePopup;
        }

        private void CreatePopup(GameObject popupPrefab)
        {
            GameObject mainCanvas = GameObject.FindGameObjectWithTag("MainCanvas");
            if (mainCanvas != null)
            {
                _popup = Instantiate(popupPrefab, mainCanvas.transform);
                AddBackground();
            }
        }

        private void AddBackground()
        {
            var bgTex = new Texture2D(1, 1);
            bgTex.SetPixel(0, 0, _backgroundColor);
            bgTex.Apply();

            _background = new GameObject("PopupBackground");
            var image = _background.AddComponent<Image>();
            var rect = new Rect(0, 0, bgTex.width, bgTex.height);
            var sprite = Sprite.Create(bgTex, rect, new Vector2(0.5f, 0.5f), 1);
            image.material.mainTexture = bgTex;
            image.sprite = sprite;
            var newColor = image.color;
            image.color = newColor;
            image.canvasRenderer.SetAlpha(0.0f);
            image.CrossFadeAlpha(1.0f, 0.4f, false);

            var canvas = GameObject.Find("Canvas");
            _background.transform.localScale = new Vector3(1, 1, 1);
            _background.GetComponent<RectTransform>().sizeDelta = canvas.GetComponent<RectTransform>().sizeDelta;
            _background.transform.SetParent(canvas.transform, false);
            _background.transform.SetSiblingIndex(_popup.transform.GetSiblingIndex());
        }
    }
}