using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioManager : SingletonBase<AudioManager>
{
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSourcePrefab;

    private readonly Queue<AudioSource> sfxPool = new Queue<AudioSource>();
    private const int PoolSize = 2;

    protected override void Awake()
    {
        base.Awake();
        InitializePool();
    }

    private void InitializePool()
    {
        for (int i = 0; i < PoolSize; i++)
        {
            AudioSource sfx = Instantiate(sfxSourcePrefab, transform);
            sfx.gameObject.SetActive(false);
            sfxPool.Enqueue(sfx);
        }
    }

    public void PlayMusic(AudioClip clip, bool loop = true)
    {
        if (musicSource.clip == clip) return;

        musicSource.clip = clip;
        musicSource.loop = loop;
        musicSource.Play();
    }

    public IEnumerator FadeMusic(float targetVolume, float duration)
    {
        float startVolume = musicSource.volume;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(startVolume, targetVolume, elapsed / duration);
            yield return null;
        }

        musicSource.volume = targetVolume;
    }

    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        if (sfxPool.Count == 0) return;

        AudioSource sfx = sfxPool.Dequeue();
        sfx.gameObject.SetActive(true);
        sfx.volume = volume;
        sfx.clip = clip;
        sfx.Play();

        StartCoroutine(ReturnToPool(sfx, clip.length));
    }

    private IEnumerator ReturnToPool(AudioSource sfx, float delay)
    {
        yield return new WaitForSeconds(delay);
        sfx.gameObject.SetActive(false);
        sfxPool.Enqueue(sfx);
    }

    public void SetMusicVolume(float volume) => musicSource.volume = Mathf.Clamp01(volume);
    public void SetSFXVolume(float volume)
    {
        foreach (AudioSource sfx in sfxPool)
            sfx.volume = Mathf.Clamp01(volume);
    }
}
