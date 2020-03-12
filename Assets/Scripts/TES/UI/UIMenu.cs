using UnityEngine.SceneManagement;

namespace TES3Unity.UI
{
    public class UIMenu : UIWindow
    {
        public void Resume() => NotifyCloseRequest();

        public void Quit()
        {
            SceneManager.LoadScene("Menu");
        }
    }
}
