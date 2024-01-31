using System.Collections;
using UnityEngine;



public class GameSceneController : MonoBehaviour
{

    private const float FadeInTime = 0.5f;
    private const float FadeOutTime = 0.5f;

    private bool KeysEnabled = false;

    [SerializeField] private SpriteRenderer backgroundImage;



    void Start()
    {
        // Stop the Menu Music here just in case it has not stopped yet
        AudioManager.One.StopMenuMusic();

        StartCoroutine(FadeInScene(FadeInTime));
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
                FadeOutGameScene(Vector3.zero);
            }
        }
    }


    #region Coroutines

    private IEnumerator FadeInScene(float duration)
    {
        Color tempColor = backgroundImage.color;
        for (float t = 0.0f; t < 1.0f; t += Time.unscaledDeltaTime / duration)
        {
            tempColor.a = Mathf.Lerp(0f, 1f, t);
            backgroundImage.color = tempColor;
            yield return null;
        }
        tempColor.a = 1f;
        backgroundImage.color = tempColor;

        KeysEnabled = true;
    }

    private IEnumerator FadeOutScene(float duration)
    {
        KeysEnabled = false;

        Color startColor = backgroundImage.color;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0f);

        for (float t = 0.0f; t < 1.0f; t += Time.unscaledDeltaTime / duration)
        {
            backgroundImage.color = Color.Lerp(startColor, endColor, t);
            yield return null;
        }

        backgroundImage.color = endColor;

        GameManager.One.GameOver();
    }

    #endregion


    public void FadeOutGameScene(Vector3 portalPosition)
    {
        GameManager.One.FadeOutGameMusic();

        StartCoroutine(FadeOutScene(FadeOutTime));
    }


    private void OnEnable()
    {
        EventManager.OnPortalReached += FadeOutGameScene;
    }

    private void OnDisable()
    {
        EventManager.OnPortalReached -= FadeOutGameScene;
    }
}
