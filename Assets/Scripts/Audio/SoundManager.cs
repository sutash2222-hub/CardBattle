using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;
    public AudioSource uiSource;

    [Header("Music Clips")]
    public AudioClip mainMenuMusic;
    public AudioClip battleMusic;
    public AudioClip victoryMusic;
    public AudioClip defeatMusic;

    [Header("SFX Clips")]
    public AudioClip cardPlaySound;
    public AudioClip cardDrawSound;
    public AudioClip cardDiscardSound;
    public AudioClip attackSound;
    public AudioClip healSound;
    public AudioClip abilitySound;
    public AudioClip turnEndSound;
    public AudioClip roundEndSound;

    [Header("UI Clips")]
    public AudioClip buttonClickSound;
    public AudioClip hoverSound;
    public AudioClip menuOpenSound;
    public AudioClip menuCloseSound;

    [Header("Settings")]
    [Range(0f, 1f)]
    public float musicVolume = 0.5f;
    [Range(0f, 1f)]
    public float sfxVolume = 0.7f;
    [Range(0f, 1f)]
    public float uiVolume = 0.6f;

    private Dictionary<string, AudioClip> sfxClips = new Dictionary<string, AudioClip>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeClips();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeClips()
    {
        if (cardPlaySound != null) sfxClips["cardPlay"] = cardPlaySound;
        if (cardDrawSound != null) sfxClips["cardDraw"] = cardDrawSound;
        if (cardDiscardSound != null) sfxClips["cardDiscard"] = cardDiscardSound;
        if (attackSound != null) sfxClips["attack"] = attackSound;
        if (healSound != null) sfxClips["heal"] = healSound;
        if (abilitySound != null) sfxClips["ability"] = abilitySound;
        if (turnEndSound != null) sfxClips["turnEnd"] = turnEndSound;
        if (roundEndSound != null) sfxClips["roundEnd"] = roundEndSound;
    }

    public void PlayMusic(AudioClip clip, float fadeTime = 1f)
    {
        if (musicSource == null || clip == null) return;

        if (musicSource.isPlaying)
        {
            StartCoroutine(FadeOutMusic(fadeTime, () =>
            {
                musicSource.clip = clip;
                musicSource.volume = musicVolume;
                musicSource.Play();
                StartCoroutine(FadeInMusic(fadeTime));
            }));
        }
        else
        {
            musicSource.clip = clip;
            musicSource.volume = 0f;
            musicSource.Play();
            StartCoroutine(FadeInMusic(fadeTime));
        }
    }

    public void StopMusic(float fadeTime = 1f)
    {
        if (musicSource != null)
        {
            StartCoroutine(FadeOutMusic(fadeTime, () =>
            {
                musicSource.Stop();
            }));
        }
    }

    public void PlaySFX(string clipName)
    {
        if (sfxClips.ContainsKey(clipName))
        {
            sfxSource?.PlayOneShot(sfxClips[clipName], sfxVolume);
        }
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(clip, sfxVolume);
        }
    }

    public void PlayUI(AudioClip clip)
    {
        if (clip != null && uiSource != null)
        {
            uiSource.PlayOneShot(clip, uiVolume);
        }
    }

    public void PlayButtonClick()
    {
        PlayUI(buttonClickSound);
    }

    public void PlayHover()
    {
        PlayUI(hoverSound);
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        if (musicSource != null)
        {
            musicSource.volume = musicVolume;
        }
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
    }

    public void SetUIVolume(float volume)
    {
        uiVolume = Mathf.Clamp01(volume);
        PlayerPrefs.SetFloat("UIVolume", uiVolume);
    }

    public void LoadVolumeSettings()
    {
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 0.7f);
        uiVolume = PlayerPrefs.GetFloat("UIVolume", 0.6f);
    }

    private System.Collections.IEnumerator FadeInMusic(float duration)
    {
        float startVolume = 0f;
        float targetVolume = musicVolume;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(startVolume, targetVolume, elapsed / duration);
            yield return null;
        }

        musicSource.volume = targetVolume;
    }

    private System.Collections.IEnumerator FadeOutMusic(float duration, System.Action onComplete)
    {
        float startVolume = musicSource.volume;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / duration);
            yield return null;
        }

        musicSource.volume = 0f;
        onComplete?.Invoke();
    }
}
