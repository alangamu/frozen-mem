using Assets.Scripts.ScriptableObjects;
using Assets.Scripts.ScriptableObjects.Variables;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class SettingsManager : MonoBehaviour
    {
        [SerializeField]
        private Text _playerNameText;
        [SerializeField]
        private StringVariable _playerName;
        [SerializeField]
        private IntVariable _avatarIndexVariable;
        [SerializeField]
        private AvatarModel _avatarModel;
        [SerializeField]
        private Image _avatarImage;
        [SerializeField]
        private InputField _playerNameInputField;
        [SerializeField]
        private Slider _volumeSlider;
        [SerializeField]
        private FloatVariable _musicVolumeVariable;
        [SerializeField]
        private Slider _soundFxSlider;
        [SerializeField]
        private FloatVariable _soundFxVolumeVariable;

        public void PressLeftArrow()
        {
            int nextIndex = _avatarIndexVariable.Value - 1;
            if (nextIndex < 0)
            {
                nextIndex = _avatarModel.Avatars.Length - 1;
            }

            _avatarIndexVariable.SetValue(nextIndex);

            ShowAvatar();
        }

        public void PressRightArrow()
        {
            int nextIndex = _avatarIndexVariable.Value + 1;
            if (nextIndex == _avatarModel.Avatars.Length)
            {
                nextIndex = 0;
            }

            _avatarIndexVariable.SetValue(nextIndex);
            ShowAvatar();
        }

        public void SaveSettings()
        {
            _playerName.SetValue(_playerNameText.text);
        }

        private void OnEnable()
        {
            ShowAvatar();
            _playerNameInputField.text = _playerName.Value;
            _volumeSlider.onValueChanged.AddListener(ChangeMusicVolume);
            _volumeSlider.value = _musicVolumeVariable.Value;
            _soundFxSlider.onValueChanged.AddListener(ChangeSoundFxVolume);
            _soundFxSlider.value = _soundFxVolumeVariable.Value;
        }

        private void OnDisable()
        {
            _volumeSlider.onValueChanged.RemoveAllListeners();
            _soundFxSlider.onValueChanged.RemoveAllListeners();
        }

        private void ChangeMusicVolume(float newValue)
        {
            _musicVolumeVariable.SetValue(newValue);
        }

        private void ChangeSoundFxVolume(float newValue)
        {
            _soundFxVolumeVariable.SetValue(newValue);
        }

        private void ShowAvatar()
        {
            _avatarImage.sprite = _avatarModel.Avatars[_avatarIndexVariable.Value];
        }
    }
}