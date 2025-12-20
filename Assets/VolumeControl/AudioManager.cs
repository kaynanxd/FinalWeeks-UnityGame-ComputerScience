using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{

    public static AudioManager instance;


    public const string MUSIC_VOLUME_KEY = "MusicVolume";
    public const string SFX_VOLUME_KEY = "SFXVolume";


    [Header("Mixer")]
    [SerializeField] private AudioMixer masterMixer;

    [Header("Mixer Groups (NOVOS!)")]

    [SerializeField] private AudioMixerGroup musicMixerGroup;
    [SerializeField] private AudioMixerGroup sfxMixerGroup;

    [Header("Fontes de Áudio (AudioSources)")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    private void Awake()
    {

        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        

        SetupAudioSources();


        LoadVolumes();
    }

    private void SetupAudioSources()
    {

        if (musicSource != null)
        {
            musicSource.loop = true;
            musicSource.playOnAwake = false;

            if (musicMixerGroup != null)
                musicSource.outputAudioMixerGroup = musicMixerGroup;
        }

        if (sfxSource != null)
        {
            sfxSource.loop = false;
            sfxSource.playOnAwake = false;

            if (sfxMixerGroup != null)
                sfxSource.outputAudioMixerGroup = sfxMixerGroup;
        }
    }



    private void LoadVolumes()
    {

        float savedMusicVol = PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY, 1.0f);
        ApplyVolume(savedMusicVol, "MusicVolume");


        float savedSFXVol = PlayerPrefs.GetFloat(SFX_VOLUME_KEY, 1.0f);
        ApplyVolume(savedSFXVol, "SFXVolume");
    }

    private void ApplyVolume(float volumeLinear, string exposedParameterName)
    {
        float dbVolume;
        if (volumeLinear <= 0.0001f)
            dbVolume = -80f;
        else
            dbVolume = Mathf.Log10(volumeLinear) * 20; 

        if (masterMixer != null)
            masterMixer.SetFloat(exposedParameterName, dbVolume);
    }


    public void SetMusicVolume(float volumeLinear)
    {
        ApplyVolume(volumeLinear, "MusicVolume");
        PlayerPrefs.SetFloat(MUSIC_VOLUME_KEY, volumeLinear);
    }

    public void SetSFXVolume(float volumeLinear)
    {
        ApplyVolume(volumeLinear, "SFXVolume");
        PlayerPrefs.SetFloat(SFX_VOLUME_KEY, volumeLinear);
    }


    public void PlayMusic(AudioClip musicClip)
    {
        if (musicSource.isPlaying && musicSource.clip == musicClip) return;
        musicSource.clip = musicClip;
        musicSource.Play();
    }
    
    public void PauseMusic() { if (musicSource.isPlaying) musicSource.Pause(); }
    public void ResumeMusic() { if (!musicSource.isPlaying) musicSource.UnPause(); }
    public void StopMusic() { musicSource.Stop(); }

    public void PlaySFX(AudioClip sfxClip)
    {
        sfxSource.PlayOneShot(sfxClip);
    }
}