using UnityEngine;
using UnityEngine.Audio;

public class VolumeControl : MonoBehaviour
{
    [SerializeField] AudioMixer _mixer;

    public void SetVolume(float volume)
    {
        _mixer.SetFloat("Volume", volume);
        PlayerPrefs.SetFloat("Volume", volume);
    }

    void Start()
    {
        float volume = PlayerPrefs.GetFloat("Volume", -20);
        _mixer.SetFloat("Volume", volume);
    }
}
