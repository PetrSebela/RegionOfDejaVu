using System;
using UnityEngine;
using UnityEngine.Rendering;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public float LoopTime;    
    public float LoopTimeCountdown;
    public bool Slowmotion = false;
    [SerializeField] GameObject playerObject;
    [SerializeField] TimeJump _jumpDrive;
    [SerializeField] ParticleSystem deathPartices; 
    [SerializeField] CanvasGroup _deathScreen;
    [SerializeField] CanvasGroup _gameUI;
    [SerializeField] Volume _deathVolume;
    private Action<float> uiFadeCallback;

    void Start()
    {
        uiFadeCallback += SetUIFade;
        Instance = this;
        Ressurect();
    }

    void Update()
    {
        // Too lazy to create separate script for tracking
        deathPartices.gameObject.transform.position = playerObject.transform.position;
    }

    public void Ressurect()
    {
        playerObject.SetActive(true);
        playerObject.transform.position = transform.position;
        _jumpDrive.Setup();
        LeanTween.cancel(this.gameObject);
        LeanTween.value(this.gameObject, uiFadeCallback, _deathScreen.alpha , 0, 0.5f);
    }

    public void PlayerDied()
    {
        deathPartices.Play();
        playerObject.SetActive(false);
        _jumpDrive.Disable();
        LeanTween.cancel(this.gameObject);
        LeanTween.value(this.gameObject, uiFadeCallback, _deathScreen.alpha , 1, 0.5f);
    }

    public void SetUIFade(float fadeValue)
    {
        _gameUI.alpha = 1 - fadeValue;
        _deathScreen.alpha = fadeValue;
        _deathVolume.weight = fadeValue;
    }
}
