using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;



/// <summary>
/// Handles the main menu interactions.
/// </summary>
public class MainMenuController : MonoBehaviour
{
    private const float FadeInTime = 0.5f;
    private const float FadeOutTime = 0.5f;

    private string sceneToLoad = "";

    private bool KeysEnabled = false;

    [SerializeField] private Image backgroundImage;
    [SerializeField] private Button playButton;
    [SerializeField] private Button testSceneButton;
    [SerializeField] private Text playButtonText;
    [SerializeField] private Text testSceneButtonText;


    private void Start()
    {
        // Set background initial alpha to 0
        Color tempColor = backgroundImage.color;
        tempColor.a = 0f;
        backgroundImage.color = tempColor;

        Color tempPlayBtnTextColor = playButtonText.color;
        tempPlayBtnTextColor.a = 0f;
        playButtonText.color = tempPlayBtnTextColor;


        // Add button click listeners
        playButton.onClick.AddListener(OnPlayButtonClicked);
        testSceneButton.onClick.AddListener(OnTestSceneButtonClicked);

        // Stop the Game Music here just in case it has not stopped yet
        AudioManager.One.StopGameMusic();

        // Fade in Main Menu music
        AudioManager.One.FadeInMenuMusic(FadeInTime);

        KeysEnabled = false;

        // Fade in Main menu
        StartCoroutine(FadeInMainMenu(backgroundImage, FadeInTime));
    }


    private void Update()
    {
        if (KeysEnabled)
        {
            if (Input.GetKeyDown(KeyCode.M))
            {
                AudioManager.One.ToggleMusicVolume();
            }
            else if (Input.GetKeyDown(KeyCode.Escape))
            {
                AudioManager.One.FadeOutMenuMusic(FadeOutTime);
                StartCoroutine(FadeOutQuit(backgroundImage, FadeOutTime));
            }
            else if (Input.GetKeyDown(KeyCode.Space))
            {
                OnPlayButtonClicked();
            }
        }
    }


    private void SetButtonsInteractable(bool interactable)
    {
        playButton.interactable = interactable;
        KeysEnabled = interactable;
    }


    private void OnPlayButtonClicked()
    {
        sceneToLoad = "GameScene";

        AudioManager.One.PlayButtonClickSFX();

        AudioManager.One.FadeOutMenuMusic(FadeOutTime);
        StartCoroutine(FadeOutMainMenu(backgroundImage, FadeOutTime));
    }
    
    private void OnTestSceneButtonClicked()
    {
        sceneToLoad = "TestScene";

        AudioManager.One.PlayButtonClickSFX();

        AudioManager.One.FadeOutMenuMusic(FadeOutTime);
        StartCoroutine(FadeOutMainMenu(backgroundImage, FadeOutTime));
    }

    private void OnQuitKeyPressed()
    {
        AudioManager.One.PlayButtonClickSFX();

        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }


    #region Coroutines

    private IEnumerator FadeInMainMenu(Image image, float duration)
    {
        yield return new WaitForSecondsRealtime(0.2f);

        SetButtonsInteractable(false);

        Color tempColor = image.color;
        Color tempPlayBtnTextColor = playButtonText.color;
        for (float t = 0.0f; t < 1.0f; t += Time.unscaledDeltaTime / duration)
        {
            tempColor.a = Mathf.Lerp(0f, 1f, t);
            image.color = tempColor;

            tempPlayBtnTextColor.a = Mathf.Lerp(0f, 1f, t);
            playButtonText.color = tempPlayBtnTextColor;

            yield return null;
        }

        tempColor.a = 1f;
        image.color = tempColor;

        SetButtonsInteractable(true);
    }


    private IEnumerator FadeOutMainMenu(Image image, float duration)
    {
        SetButtonsInteractable(false);

        Color startColor = image.color;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0f);

        Color textStartColor = playButtonText.color;
        Color textEndColor = new Color(textStartColor.r, textStartColor.g, textStartColor.b, 0f);

        for (float t = 0.0f; t < 1.0f; t += Time.unscaledDeltaTime / duration)
        {
            image.color = Color.Lerp(startColor, endColor, t);
            playButtonText.color = Color.Lerp(textStartColor, textEndColor, t);  

            yield return null;
        }

        image.color = endColor;
        playButtonText.color = textEndColor;  

        // Load the level after the screen has faded out
        SceneManager.LoadScene(sceneToLoad);
    }

    private IEnumerator FadeOutQuit(Image image, float duration)
    {
        SetButtonsInteractable(false);

        Color startColor = image.color;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0f);

        Color textStartColor = playButtonText.color;
        Color textEndColor = new Color(textStartColor.r, textStartColor.g, textStartColor.b, 0f);

        for (float t = 0.0f; t < 1.0f; t += Time.unscaledDeltaTime / duration)
        {
            image.color = Color.Lerp(startColor, endColor, t);
            playButtonText.color = Color.Lerp(textStartColor, textEndColor, t);

            yield return null;
        }

        image.color = endColor;
        playButtonText.color = textEndColor;

        OnQuitKeyPressed();
    }

    #endregion


    private void OnDestroy()
    {
        // Remove button click listeners
        playButton.onClick.RemoveListener(OnPlayButtonClicked);
        testSceneButton.onClick.RemoveListener(OnTestSceneButtonClicked);
    }

}