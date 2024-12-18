using System;
using UnityEngine;
using UnityEngine.Rendering;
using TMPro;
using UnityEngine.SceneManagement;

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
    [SerializeField] CanvasGroup _gameFinishedScreen;
    [SerializeField] Volume _gameBlur;
    [SerializeField] ParticleSystem[] _timeMachineParticles;
    private Action<float> _deathScreenFadeCallback;
    private Action<float> _uiFadeCallback;
    private Action<float> _gameFinishedCallback;
    bool _freePlayMode = false;

    // Quests
    private bool _securityShutdown = false;
    [SerializeField] ButtonInteraction _securityShutdownButton;
    private bool _cryoCoolantShutdown = false;
    [SerializeField] ButtonInteraction _cryoCoolantShutdownButton;
    private bool _mainCoreShutdown = false;
    [SerializeField] ButtonInteraction _mainCoreShutdownButton;
    
    public bool LoopRunning = false;
    [SerializeField] ButtonInteraction doorOpen;
    [SerializeField] GameObject coolantDoors;
    [SerializeField] GameObject coreDoors;
    [SerializeField] AudioSource _timeMachineSound;

    public bool Paused = false;
    [SerializeField] CanvasGroup pauseMenu;
    private Action<float> _pauseMenuFade;

    void Start()
    {
        _deathScreenFadeCallback += SetDeathScreenFade;
        _gameFinishedCallback += SetGameFinishedAlpha;
        _uiFadeCallback += SetUIAlphaCallback;
        _pauseMenuFade += PauseMenuFade;
        Instance = this;
        SetDeathScreenFade(0);
        SetGameFinishedAlpha(0);
        SetUIAlphaCallback(0);
        SpawnPlayer();
        foreach(ParticleSystem p in _timeMachineParticles)
            p.Stop();

        TogglePause();
    }
    void PauseMenuFade(float alpha)
    {
        pauseMenu.alpha = alpha;
        _gameBlur.weight = alpha;
    }

    void SetGameFinishedAlpha(float alpha)
    {
        _gameFinishedScreen.alpha = alpha;
        _gameBlur.weight = alpha;
    }
    
    void SetUIAlphaCallback(float alpha)
    {
        _gameUI.alpha = alpha;
    }

    public void Quit()
    {
        Application.Quit();
    }

    void Update()
    {
        // Too lazy to create separate script for tracking
        deathPartices.gameObject.transform.position = playerObject.transform.position;

        if(LoopRunning)
        {
            LoopTime -= Time.deltaTime;
            int minutes = (int)(LoopTime / 60);
            int seconds = (int)(LoopTime - minutes * 60);
            time_text.text = string.Format("{0}:{1}", minutes, seconds);

            if (LoopTime <= 0)
            {
                LoopTime = 0;
                TimeRewind();
            }
        }
    }

    public void TimeRewind()
    {
        LoopTime = LoopTimeCountdown;
        playerObject.SetActive(true);
        playerObject.transform.position = transform.position;
        _jumpDrive.Setup();
        LeanTween.cancel(this.gameObject);
        LeanTween.value(this.gameObject, _deathScreenFadeCallback, _deathScreen.alpha , 0, 0.5f);
        _jumpDrive.ParticleEffects.Play();
    }

    public void SpawnPlayer()
    {
        LoopTime = LoopTimeCountdown;
        playerObject.SetActive(true);
        playerObject.transform.position = transform.position;
        _jumpDrive.Setup();
    }

    public void PlayerDied()
    {
        deathPartices.Play();
        playerObject.SetActive(false);
        _jumpDrive.Disable();
        LeanTween.cancel(this.gameObject);
        LeanTween.value(this.gameObject, _deathScreenFadeCallback, _deathScreen.alpha , 1, 0.5f);
    }

    public void SetDeathScreenFade(float fadeValue)
    {
        _gameUI.alpha = 1 - fadeValue;
        _deathScreen.alpha = fadeValue;
        _gameBlur.weight = fadeValue;
    }

    public void TogglePause()
    {
        Paused = !Paused;

        if(Paused)
        {
            LeanTween.value(this.gameObject, _pauseMenuFade, pauseMenu.alpha, 1, 0.125f).setIgnoreTimeScale(true);
            pauseMenu.interactable = true;
            pauseMenu.blocksRaycasts = true;
            Time.timeScale = 0;
        }
        else
        {
            LeanTween.value(this.gameObject, _pauseMenuFade, pauseMenu.alpha, 0, 0.125f).setIgnoreTimeScale(true);
            pauseMenu.interactable = false;
            pauseMenu.blocksRaycasts = false;
            Time.timeScale = 1;
        }
    }

#region  StoryPoints
    public void LaunchExperiment()
    {
        if(_freePlayMode)
            return;
        Debug.Log("Story: Experiment launched");
        LoopRunning = true;
        doorOpen.enabled = true;
        _securityShutdownButton.enabled = true;
        LeanTween.value(this.gameObject, _uiFadeCallback, _gameUI.alpha , 1, 0.5f);
        
        foreach(ParticleSystem p in _timeMachineParticles)
            p.Play();

        _timeMachineSound.Play();
    } 

    

    public void SecurityShutdown()
    {
        Debug.Log("Story: Security shutdown");
        _securityShutdown = true;
        _cryoCoolantShutdownButton.enabled = true;
        LeanTween.moveLocalY(coolantDoors, coolantDoors.transform.position.y - 5, 0.125f);
    } 

    public void ShutdownCryoCoolant()
    {
        Debug.Log("Story: Cryocoolant shutdown");
        _cryoCoolantShutdown = true;
        LeanTween.moveLocalY(coreDoors, coreDoors.transform.position.y - 5, 0.125f);
        _mainCoreShutdownButton.enabled = true;
    } 

    public void MainCoreShutdown()
    {
        Debug.Log("Story: main core shutdown");
        _mainCoreShutdown = true;
        foreach(ParticleSystem p in _timeMachineParticles)
            p.Stop();
        OnGameFinished();
        _timeMachineSound.Stop();
    } 

    
    public void OnGameFinished()
    {
        LoopRunning = false;
        LeanTween.value(this.gameObject, _gameFinishedCallback, 0, 1, 0.25f);
        SetUIAlphaCallback(0);
    }

    public void FreePlay()
    {
        _freePlayMode = true;
        LeanTween.value(this.gameObject, _gameFinishedCallback, 1, 0, 0.25f);
        SetUIAlphaCallback(1);

        _mainCoreShutdownButton.enabled = false;
        _cryoCoolantShutdownButton.enabled = false;    
        _mainCoreShutdownButton.enabled = false;
    }

    public void StartNewGame()
    {
        SceneManager.LoadScene("MainGame");
    }
#endregion
}
