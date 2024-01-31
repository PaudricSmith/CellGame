using UnityEngine.SceneManagement;



/// <summary>
/// Manages the overall game logic.
/// </summary>
public class GameManager : Singleton<GameManager>
{

    private const string MainMenuSceneName = "MainMenu";
    private const string GameSceneName = "GameScene";
    private const string TestSceneName = "TestScene";
    private const float FadeInTime = 1.0f;
    private const float FadeOutTime = 1.0f;

    public static bool IsGameRunning = true;



    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }


    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == GameSceneName || scene.name == TestSceneName)
        {
            // Reset the game state
            IsGameRunning = true;  

            // Fade in game scene music
            AudioManager.One.FadeInGameMusic(FadeInTime);
        }
    }

    public void FadeOutGameMusic()
    {
        // Fade out game scene music
        AudioManager.One.FadeOutGameMusic(FadeOutTime);
    }


    /// <summary>
    /// Ends the game
    /// </summary>
    public void GameOver()
    {
        // Set game state to false
        IsGameRunning = false;

        // Load the Main Menu
        SceneManager.LoadSceneAsync(MainMenuSceneName);
    }


    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
