using UnityEngine;

public class AudioManager : SingletonBase<AudioManager>
{

    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;


    // Play background music with loop option
    public void PlayMusic(AudioClip clip, bool loop = true)
    {
        if (musicSource.clip == clip) return;

        musicSource.clip = clip;
        musicSource.loop = loop;
        musicSource.Play();
    }

    // Stop background music
    public void StopMusic()
    {
        musicSource.Stop();
    }

    // Play a single sound effect
    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        sfxSource.PlayOneShot(clip, volume);
    }

    // Volume Control
    public void SetMusicVolume(float volume)
    {
        musicSource.volume = Mathf.Clamp01(volume);
    }

    public void SetSFXVolume(float volume)
    {
        sfxSource.volume = Mathf.Clamp01(volume);
    }
}
