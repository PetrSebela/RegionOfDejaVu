using System;
using UnityEngine;
using UnityEngine.Rendering;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public float LoopTime;    
    public float LoopTimeCountdown;
    public TMP_Text time_text;
    public bool Slowmotion = false;
    [SerializeField] GameObject playerObject;
    [SerializeField] TimeJump _jumpDrive;
    [SerializeField] ParticleSystem deathPartices; 
    [SerializeField] CanvasGroup _deathScreen;
    [SerializeField] CanvasGroup _gameUI;
    [SerializeField] Volume _deathVolume;
    private Action<float> uiFadeCallback;


    // Quests
    private bool CryCoolantShutdown = false;
    private bool ExternalPowerShutdown = false;
    private bool MainCoreShutdown = false;
    
    public bool LoopRunning = false;
[SerializeField] ButtonInteraction doorOpen;

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


        if(LoopRunning)
        {
            LoopTime -= Time.deltaTime;
            int minutes = (int)(LoopTime / 60);
            int seconds = (int)(LoopTime % 60);
            time_text.text = string.Format("{0}:{1}", minutes, seconds);

            if (LoopTime <= 0)
            {
                LoopTime = 0;
                Ressurect();
            }
        }
    }

    public void LaunchExperiment()
    {
        LoopRunning = true;
        doorOpen.enabled = true;
    } 

    public void Ressurect()
    {
        LoopTime = LoopTimeCountdown;
        playerObject.SetActive(true);
        playerObject.transform.position = transform.position;
        _jumpDrive.Setup();
        LeanTween.cancel(this.gameObject);
        LeanTween.value(this.gameObject, uiFadeCallback, _deathScreen.alpha , 0, 0.5f);
        _jumpDrive.ParticleEffects.Play();
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
