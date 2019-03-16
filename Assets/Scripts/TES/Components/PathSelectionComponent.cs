using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR;

namespace TESUnity.Components
{
    public class PathSelectionComponent : MonoBehaviour
    {
        private MusicPlayer m_MusicPlayer = null;
        private string m_GamePath = string.Empty;

        [SerializeField]
        private Image m_Background = null;
        [SerializeField]
        private InputField _path = null;
        [SerializeField]
        private GameObject m_ButtonsContainer = null;
        [SerializeField]
        private Text _infoMessage = null;

        private void Awake()
        {
#if UNITY_ANDROID
            if (XRSettings.enabled)
            {
                SceneManager.LoadScene("Game");
                return;
            }
#endif

            m_GamePath = GameSettings.GetDataPath();

            if (GameSettings.IsValidPath(m_GamePath))
            {
                var path = Path.Combine(m_GamePath, "Splash");

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

                path = Path.Combine(m_GamePath, "Music", "Explore", "Morrowind Title.mp3");

                if (File.Exists(path))
                {
                    m_MusicPlayer = new MusicPlayer();
                    m_MusicPlayer.AddSong(path);
                    m_MusicPlayer.Play();
                }

                _path.gameObject.SetActive(false);
            }
        }

        public void LoadWorld()
        {
            var path = _path.text;

            if (GameSettings.IsValidPath(path))
            {
                GameSettings.CreateConfigFile();
                GameSettings.SetDataPath(path);

                StartCoroutine(LoadWorld(_path.text));
            }
            else
            {
                if (GameSettings.IsValidPath(m_GamePath))
                    StartCoroutine(LoadWorld(m_GamePath));
                else
                    StartCoroutine(ShowErrorMessage("This path is not valid."));
            }
        }

        private IEnumerator LoadWorld(string path)
        {
            _path.gameObject.SetActive(false);
            m_ButtonsContainer.gameObject.SetActive(false);
            _infoMessage.text = "Loading...";
            _infoMessage.enabled = true;

            yield return new WaitForEndOfFrame();

            var asyncOperation = SceneManager.LoadSceneAsync("Game");
            var waitForSeconds = new WaitForSeconds(0.1f);

            while (!asyncOperation.isDone)
            {
                _infoMessage.text = string.Format("Loading {0}%", Mathf.RoundToInt(asyncOperation.progress * 100.0f));
                yield return waitForSeconds;
            }
        }

        private IEnumerator ShowErrorMessage(string message)
        {
            _infoMessage.text = message;
            _infoMessage.enabled = true;
            yield return new WaitForSeconds(5.0f);
            _infoMessage.enabled = false;
        }

        public void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif

            Application.Quit();
        }
    }
}