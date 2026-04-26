using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private GameObject CreditsTab;
    [SerializeField] private GameObject StoryTab;

    private void Start()
    {
        AudioManager.Instance?.PlayMenuMusic();
    }
    public void StartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainScene");
    }

    public void ShowCredits()
    {
        CreditsTab.SetActive(true);
        StoryTab.SetActive(false);
    }

    public void ShowStory()
    {
        StoryTab.SetActive(true);
        CreditsTab.SetActive(false);
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }

    public void BackToMainMenu()
    {
        StoryTab.SetActive(false);
        CreditsTab.SetActive(false);
    }

    private void OnDisable()
    {
        AudioManager.Instance?.StopMenuMusic();
    }
}
