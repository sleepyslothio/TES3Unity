using System.Collections;
using System.IO;
using System.Threading;
using Demonixis.ToolboxV2.Inputs;
using Demonixis.ToolboxV2.XR;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;
#if UNITY_ANDROID
using UnityEngine.Android;
#endif

namespace TES3Unity.Components
{
    public sealed class MenuComponent : MonoBehaviour
    {
        private MusicPlayer _musicPlayer;
        private Thread _preloadThread;
        private string _gamePath = string.Empty;
        private bool _menuLoaded;

        [Header("Players")] [SerializeField] private Transform _playerSpawnPoint;
        [SerializeField] private PlayerPrefabData _playerPrefabData;

        [Header("Panels")] [SerializeField] private GameObject _permissionPanel;
        [SerializeField] private GameObject _preloadPanel;
        [SerializeField] private GameObject _menuPanel;
        [SerializeField] private GameObject _optionsPanel;

        [Header("UI Elements")] [SerializeField]
        private Image _background;

        [Header("Preloading")] [SerializeField]
        private GameObject _desktopPathSelection;

        [SerializeField] private GameObject _mobilePathSelection;
        [SerializeField] private Text _pathValidationText;

        [Header("Menu")] [SerializeField] private InputField _pathSelector;
        [SerializeField] private Button _loadButton;
        [SerializeField] private Text _infoMessage;
        [SerializeField] private GameObject _buttonsContainer;
        [SerializeField] private Button _loadSaveButton;

#if UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
        [Header("Editor Only")] [SerializeField]
        private bool _displayDesktopPathOnAndroid = true;
#endif

        private void Awake()
        {
            Instantiate(_playerPrefabData.GetPlayerMenuPrefab());

            if (!CanReadStorage())
            {
                RequestReadStoragePermission();
                StartCoroutine(DeferredLoad());
            }
            else
            {
                Initialize();
            }
        }

        private void Initialize()
        {
            _gamePath = GameSettings.GetDataPath();

            if (GameSettings.IsValidPath(_gamePath))
            {
                LoadMenu();
            }
            else
            {
                LoadPreloader();
            }
        }

        private IEnumerator DeferredLoad()
        {
            SetPanelVisible(-1);

            // Required for static initialization of the InputManager.
            yield return null;

            var wait = new WaitForEndOfFrame();

            var actionMap = InputSystemManager.GetActionMap("UI");
            actionMap.Enable();

            var backAction = actionMap["Menu"];

            while (!CanReadStorage())
            {
                if (backAction.phase == InputActionPhase.Started)
                {
                    Quit();
                }

                yield return wait;
            }

            Initialize();
        }

        #region PreLoading

        private void LoadPreloader()
        {
            var mobile = false;

#if UNITY_ANDROID || UNITY_IOS
            mobile = true;

#if UNITY_EDITOR
            if (_displayDesktopPathOnAndroid)
            {
                mobile = false;
            }
#endif
#endif

            _desktopPathSelection.SetActive(!mobile);
            _mobilePathSelection.SetActive(mobile);

            SetPanelVisible(0);
        }

        public void ValidatePathSelection()
        {
            _gamePath = _pathSelector.text;

            if (GameSettings.IsValidPath(_gamePath))
            {
                GameSettings.SetDataPath(_gamePath);
                _pathValidationText.enabled = false;
                LoadMenu();
            }
            else
            {
                _pathValidationText.enabled = true;
            }
        }

        #endregion

        #region Menu

        private void LoadMenu()
        {
            // Preload data if the reader is not yet initialized.
            if (TES3Engine.DataReader == null)
            {
                _preloadThread = new Thread(() => { TES3Engine.DataReader = new TES3DataReader(_gamePath); });
                _preloadThread.Start();

                StartCoroutine(CheckLoadButton());
            }

            SetPanelVisible(1);

            var path = Path.Combine(_gamePath, "Splash");

            if (Directory.Exists(path))
            {
                var files = Directory.GetFiles(path, "*.tga");
                if (files.Length > 0)
                {
                    var target = files[Random.Range(0, files.Length - 1)];
                    var texture = TGALoader.LoadTGA(target);
                    var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
                        Vector2.zero);
                    _background.sprite = sprite;
                    _background.color = Color.white;
                }
            }

            path = Path.Combine(_gamePath, "Music", "Explore", "Morrowind Title.mp3");

            if (File.Exists(path))
            {
                _musicPlayer = new MusicPlayer();
                _musicPlayer.AddSong(path);
                _musicPlayer.Play();
            }

            _menuLoaded = true;
        }

        public void LoadWorld()
        {
            TES3Engine.AutoLoadSavedGame = false;
            StartCoroutine(LoadWorld(_gamePath));
        }

        public void LoadSavedGame()
        {
            TES3Engine.AutoLoadSavedGame = true;
            LoadWorld();
        }

        private IEnumerator CheckLoadButton()
        {
            var wait = new WaitForSeconds(0.5f);

            _infoMessage.text = "Please wait while loading Morrowind's data...";
            _infoMessage.enabled = true;

            _loadButton.interactable = false;
            _loadSaveButton.interactable = false;

            while (_preloadThread.IsAlive)
            {
                yield return wait;
            }

            _loadButton.interactable = true;
            _infoMessage.enabled = false;
            _loadSaveButton.interactable = !TES3Save.Get().IsEmpty();

#if UNITY_STANDALONE
            if (!XRManager.IsXREnabled())
            {
                var textureManager = new TextureManager(TES3Engine.DataReader);
                var texture = textureManager.LoadTexture("tx_cursor", true);
                Cursor.SetCursor(texture, Vector2.zero, CursorMode.Auto);
            }
#endif
        }

        private IEnumerator LoadWorld(string path)
        {
            _buttonsContainer.gameObject.SetActive(false);
            _infoMessage.text = "Loading...";
            _infoMessage.enabled = true;

            yield return new WaitForEndOfFrame();

            var asyncOperation = SceneManager.LoadSceneAsync("Game");
            var waitForSeconds = new WaitForSeconds(0.1f);

            while (!asyncOperation.isDone)
            {
                _infoMessage.text = $"Loading {Mathf.RoundToInt(asyncOperation.progress * 100.0f)}%";
                yield return waitForSeconds;
            }
        }

        public void Quit()
        {
            var dataReader = TES3Engine.DataReader;
            dataReader?.Close();

#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#endif
            Application.Quit();
        }

        #endregion

        #region Options

        public void ShowOptions(bool visible)
        {
            if (visible)
            {
                SetPanelVisible(2);
            }
            else
            {
                SetPanelVisible(1);
            }
        }

        #endregion

        private void RequestReadStoragePermission()
        {
#if UNITY_ANDROID
            Permission.RequestUserPermission(Permission.ExternalStorageRead);
#endif
        }

        private bool CanReadStorage()
        {
#if UNITY_ANDROID
            return Permission.HasUserAuthorizedPermission(Permission.ExternalStorageRead);
#else
            return true;
#endif
        }

        public bool CanLoadWorld()
        {
            return _menuLoaded && (_preloadThread == null || !_preloadThread.IsAlive);
        }

        public void SetPanelVisible(int index)
        {
            _permissionPanel.SetActive(index == -1);
            _preloadPanel.SetActive(index == 0);
            _menuPanel.SetActive(index == 1);
            _optionsPanel.SetActive(index == 2);
        }

        private IEnumerator ShowMessage(string message, float duration)
        {
            _infoMessage.text = message;
            _infoMessage.enabled = true;
            yield return new WaitForSeconds(duration);
            _infoMessage.enabled = false;
        }
    }
}