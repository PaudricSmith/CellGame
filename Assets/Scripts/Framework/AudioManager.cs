using System.Collections;
using UnityEngine;



/// <summary>
/// Manages all audio functionalities.
/// </summary>
public class AudioManager : Singleton<AudioManager>
{

    [SerializeField] private AudioClip menuMusic;
    [SerializeField] private AudioClip gameMusic;
    [SerializeField] private AudioClip buttonClickSFX;
    [SerializeField] private AudioClip playerFootstepSFX;
    [SerializeField] private AudioClip playerLandSFX;
    [SerializeField] private AudioClip upgradeSFX;
    [SerializeField] private AudioClip endPortalSFX;
    [SerializeField] private AudioSource gameMusicSource;
    [SerializeField] private AudioSource menuMusicSource;
    [SerializeField] private AudioSource sfxAudioSource;
    [SerializeField] private AudioSource playerSfxAudioSource;



    #region SFX

    /// <summary>
    /// Plays the SFX audio clip.
    /// </summary>
    public void PlaySFX(AudioClip sfxClip)
    {
        sfxAudioSource.PlayOneShot(sfxClip);
    }

    /// <summary>
    /// Plays the SFX audio clip delayed by the time parameter.
    /// </summary>
    public void PlayDelayedSFX(AudioClip sfxClip, float delayTime)
    {
        StartCoroutine(PlaySFXWithDelay(sfxClip, delayTime));
    }

    /// <summary>
    /// Plays the button click sound effect.
    /// </summary>
    public void PlayButtonClickSFX()
    {
        sfxAudioSource.PlayOneShot(buttonClickSFX);
    }


    public void PlayUpgradeSound()
    {
        if (!sfxAudioSource.isPlaying)
        {
            sfxAudioSource.PlayOneShot(upgradeSFX);
        }
    }

    public void PlayEndPortalSound()
    {
        if (!sfxAudioSource.isPlaying)
        {
            sfxAudioSource.PlayOneShot(endPortalSFX);
        }
    }

    public void PlayLandSound()
    {
        if (!playerSfxAudioSource.isPlaying)
        {
            playerSfxAudioSource.PlayOneShot(playerLandSFX);
        }
    }

    public void PlayWalkingSound()
    {
        if (!playerSfxAudioSource.isPlaying)        
        {
            playerSfxAudioSource.PlayOneShot(playerFootstepSFX);
        }
    }

    public void StopPlayerSound()
    {
        playerSfxAudioSource.Stop();
    }


    private IEnumerator PlaySFXWithDelay(AudioClip sfxClip, float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        sfxAudioSource.PlayOneShot(sfxClip);
    }

    #endregion



    #region MUSIC

    /// <summary>
    /// Mute and unmute main menu music.
    /// </summary>
    public void ToggleMusicVolume()
    {
        if (menuMusicSource.volume > 0.0f || gameMusicSource.volume > 0.0f)
        {
            menuMusicSource.volume = 0.0f;
            gameMusicSource.volume = 0.0f;
        }
        else
        {
            menuMusicSource.volume = 0.2f;
            gameMusicSource.volume = 0.2f;
        }
    }

    /// <summary>
    /// Fades in menu music over a specified duration.
    /// </summary>
    /// <param name="duration">Duration of the fade-in effect.</param>
    public void FadeInMenuMusic(float duration)
    {
        StartCoroutine(FadeInProcess(menuMusicSource, menuMusic, duration));
    }

    /// <summary>
    /// Fades out menu music over a specified duration.
    /// </summary>
    /// <param name="duration">Duration of the fade-out effect.</param>
    public void FadeOutMenuMusic(float duration)
    {
        StartCoroutine(FadeOutProcess(menuMusicSource, duration));
    }

    /// <summary>
    /// Stop the game music.
    /// </summary>
    public void StopGameMusic()
    {
        gameMusicSource.Stop();
    }

    /// <summary>
    /// Stop the menu music.
    /// </summary>
    public void StopMenuMusic()
    {
        menuMusicSource.Stop();
    }

    /// <summary>
    /// Fades in game music over a specified duration.
    /// </summary>
    /// <param name="duration">Duration of the fade-in effect.</param>
    public void FadeInGameMusic(float duration)
    {
        StartCoroutine(FadeInProcess(gameMusicSource, gameMusic, duration));
    }

    /// <summary>
    /// Fades out game music over a specified duration.
    /// </summary>
    /// <param name="duration">Duration of the fade-out effect.</param>
    public void FadeOutGameMusic(float duration)
    {
        StartCoroutine(FadeOutProcess(gameMusicSource, duration));
    }

    #endregion


    private IEnumerator FadeInProcess(AudioSource audioSource, AudioClip audioClip, float duration)
    {
        yield return new WaitForSecondsRealtime(0.1f);

        if (audioClip)
        {
            audioSource.clip = audioClip;
            audioSource.Play();
        }

        // Store the initial volume
        float startVolume = audioSource.volume;

        // Initialize volume to zero
        audioSource.volume = 0;

        // Gradually increase volume to target volume
        while (audioSource.volume < startVolume)
        {
            // Incrementally increase the volume of the audio source based on the target volume, frame time, and duration
            audioSource.volume = Mathf.MoveTowards(audioSource.volume, startVolume, (startVolume / duration) * Time.unscaledDeltaTime);

            // Pause the coroutine and wait for next frame before continuing
            yield return null;
        }

        // Reset the volume to the initial value
        audioSource.volume = startVolume;
    }

    private IEnumerator FadeOutProcess(AudioSource audioSource, float duration)
    {
        // Store the initial volume
        float startVolume = audioSource.volume;

        // Gradually reduce the volume to zero
        while (audioSource.volume > 0)
        {
            audioSource.volume = Mathf.MoveTowards(audioSource.volume, 0, (startVolume / duration) * Time.unscaledDeltaTime);

            yield return null;
        }

        // Stop and reset the volume to the initial value
        audioSource.Stop();
        audioSource.volume = startVolume;
    }

}