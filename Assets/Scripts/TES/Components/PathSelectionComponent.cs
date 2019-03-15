using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace TESUnity.Components
{
    public class PathSelectionComponent : MonoBehaviour
    {
        [SerializeField]
        private Image m_Background = null;
        [SerializeField]
        private InputField _path = null;
        [SerializeField]
        private Button _button = null;
        [SerializeField]
        private Text _infoMessage = null;

        private void Awake()
        {
#if UNITY_ANDROID
            SceneManager.LoadScene("GameScene");
#else
            var savedPath = GameSettings.GetDataPath();
            if (savedPath != string.Empty)
            {
                var path = Path.Combine(savedPath, "Splash");
                if (Directory.Exists(path))
                {
                    var files = Directory.GetFiles(path);
                    if (files.Length > 0)
                    {
                        var target = files[Random.Range(0, files.Length - 1)];
                        var texture = TGALoader.LoadTGA(target);
                        var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
                        m_Background.sprite = sprite;
                        m_Background.color = Color.white;
                    }
                }
                Debug.Log("Try Loading the game.");
                StartCoroutine(LoadWorld(savedPath));
            }
            else
                Debug.Log("Bad game data path.");
#endif
        }

        public void LoadWorld()
        {
            var path = _path.text;

            if (GameSettings.IsValidPath(path))
            {
                GameSettings.CreateConfigFile();
                GameSettings.SetDataPath(path);

                StartCoroutine(LoadWorld(_path.text, false));
            }
            else
                StartCoroutine(ShowErrorMessage("This path is not valid."));
        }

        private IEnumerator LoadWorld(string path, bool checkPath = true)
        {
            if (!checkPath || GameSettings.IsValidPath(path))
            {
                _path.gameObject.SetActive(false);
                _button.gameObject.SetActive(false);
                _infoMessage.text = "Loading...";
                _infoMessage.enabled = true;

                yield return new WaitForEndOfFrame();

                var asyncOperation = SceneManager.LoadSceneAsync("GameScene");
                var waitForSeconds = new WaitForSeconds(0.1f);

                while (!asyncOperation.isDone)
                {
                    _infoMessage.text = string.Format("Loading {0}%", Mathf.RoundToInt(asyncOperation.progress * 100.0f));
                    yield return waitForSeconds;
                }
            }
            else
                StartCoroutine(ShowErrorMessage("Invalid path."));
        }

        private IEnumerator ShowErrorMessage(string message)
        {
            _infoMessage.text = message;
            _infoMessage.enabled = true;
            yield return new WaitForSeconds(5.0f);
            _infoMessage.enabled = false;
        }
    }
}