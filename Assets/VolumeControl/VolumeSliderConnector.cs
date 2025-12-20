using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class VolumeSliderConnector : MonoBehaviour
{

    public enum VolumeType { Music, SFX }
    
    [Header("Tipo de Volume")]
    [Tooltip("Este slider controla a Música ou os Efeitos Sonoros (SFX)?")]
    public VolumeType volumeType;

    private Slider volumeSlider;

    void Start()
    {
        volumeSlider = GetComponent<Slider>();
        float savedVolume = 1.0f;


        if (volumeType == VolumeType.Music)
        {
            savedVolume = PlayerPrefs.GetFloat(AudioManager.MUSIC_VOLUME_KEY, 1.0f);
        }
        else 
        {
            savedVolume = PlayerPrefs.GetFloat(AudioManager.SFX_VOLUME_KEY, 1.0f);
        }
        
        volumeSlider.value = savedVolume;


        volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
    }


    public void OnVolumeChanged(float value)
    {
        if (AudioManager.instance == null) return;


        if (volumeType == VolumeType.Music)
        {
            AudioManager.instance.SetMusicVolume(value);
        }
        else 
        {
            AudioManager.instance.SetSFXVolume(value);
        }
    }

    private void OnDestroy()
    {
        if (volumeSlider != null)
        {
            volumeSlider.onValueChanged.RemoveListener(OnVolumeChanged);
        }
    }
}