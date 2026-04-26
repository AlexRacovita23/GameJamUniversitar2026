using UnityEngine;

public class EndScreenManager : MonoBehaviour
{
    public void LoadMainMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }

    public void RestartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainScene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
