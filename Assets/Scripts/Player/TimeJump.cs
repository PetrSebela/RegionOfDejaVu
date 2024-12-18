using System;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class TimeJump : MonoBehaviour
{
    [SerializeField] private float _loopLength;
    [SerializeField] private float _leapStrength;
    [SerializeField] private float _leapStrengthLimit;
    private Vector3[] _loopSamples;
    private int _loopSampleCount = 0;
    private Rigidbody2D _rb;
    private Action<float> _tweenCallback;
    [SerializeField] private Rigidbody2D _visualization;
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private float _numbTime;
    [SerializeField] private float _slowmoTimeMax = 3;
    [SerializeField] private float _slowmoTime = 0;
    [SerializeField] private bool _inSlowmotion = false;
    [SerializeField] private bool _jumpDriveAvialable = true;
    [SerializeField] private bool _reseted = true;
    [SerializeField] private float _cooldownTime = 0.7f;
    [SerializeField] private float _timeSinceLastUse = 0;
    [SerializeField] private bool Active = true;
    [SerializeField] private SpriteRenderer _sr;
    [SerializeField] private Light2D _aura;
    [SerializeField] private Volume _effectVolume;
    [SerializeField] public ParticleSystem ParticleEffects;
    private Action<float> _effectCallback;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _loopSampleCount = (int)(_loopLength / Time.fixedDeltaTime);
        _tweenCallback += SetTimeScaleValue;
        _effectCallback += SetEffectStrength;
        _loopSamples = new Vector3[_loopSampleCount];
        for (int i = 0; i < _loopSampleCount; i++)
            _loopSamples[i] = transform.position;

        ResetJumpDrive();
    }

    void SetVisualStatus(bool status)
    {
        Active = status;
        _aura.enabled = status;
        _sr.enabled = status;
    }

    void Update()
    {
        if(_inSlowmotion )
            _slowmoTime -= Time.unscaledDeltaTime;
        else
            _slowmoTime += Time.unscaledDeltaTime * 1.25f;
        _slowmoTime = Mathf.Clamp(_slowmoTime, 0, _slowmoTimeMax);
        _timeSinceLastUse += Time.unscaledDeltaTime;
        
        if(_slowmoTime <= 0)
            DisableSlowmotion();

        _sr.enabled = _jumpDriveAvialable && Active;
    }
    void FixedUpdate()
    {
        UpdateSamples();
        _visualization.MovePosition(_loopSamples[0]);
    }

    void UpdateSamples()
    {
        if(Active)
        {
            for (int i = 0; i < _loopSampleCount - 1; i++)
                _loopSamples[i] = _loopSamples[i + 1];
            _loopSamples[_loopSampleCount - 1] = transform.position;
        }
        else
        {
            for (int i = 0; i < _loopSampleCount; i++)
                _loopSamples[i] = transform.position;
        }
    }

    public void EnableSlowmotion(InputAction.CallbackContext ctx)
    {
        _inSlowmotion = true;
        GameManager.Instance.Slowmotion = true;
        LeanTween.cancel(this.gameObject);
        LeanTween.value(this.gameObject, _tweenCallback, Time.timeScale, 0.25f, 0.125f);
    }
    public void SlowmotionProxy(InputAction.CallbackContext ctx)
    {
        DisableSlowmotion();
    }
    public void DisableSlowmotion()
    {
        _inSlowmotion = false;
        GameManager.Instance.Slowmotion = false;
        LeanTween.cancel(this.gameObject);
        LeanTween.value(this.gameObject, _tweenCallback, Time.timeScale, 1, 0.125f);
    }

    public void ResetJumpDrive()
    {
        if(_reseted || _timeSinceLastUse < _cooldownTime)
            return;
        _jumpDriveAvialable = true;
        _reseted = true;
        SetVisualStatus(true);
        for (int i = 0; i < _loopSampleCount; i++)
            _loopSamples[i] = transform.position;
    }
    private void SetEffectStrength(float strength)
    {
        Debug.Log(strength);
        _effectVolume.weight = strength;
    }
    public void Setup()
    {
        _jumpDriveAvialable = true;
        _reseted = true;
        SetVisualStatus(true);
        for (int i = 0; i < _loopSampleCount; i++)
            _loopSamples[i] = transform.position;
    }
    public void PerformTimeLeap(InputAction.CallbackContext ctx)
    {
        if(!_jumpDriveAvialable || !Active || GameManager.Instance.Paused)
            return;
        Vector3 leapDirection = _loopSamples[0] - transform.position;
        leapDirection = Vector3.ClampMagnitude(leapDirection * _leapStrength, _leapStrengthLimit);
        LeanTween.value(this.gameObject, _playerController.UpdateControlAuthority, 0, 1, _numbTime).setEaseOutCubic();
        LeanTween.value(this.gameObject, _effectCallback, 1, 0, _numbTime);
        _rb.linearVelocity = leapDirection;
        _rb.MovePosition(_loopSamples[0]);
        DisableSlowmotion();
        _jumpDriveAvialable = false;
        _reseted = false;
        SetVisualStatus(false);
        _timeSinceLastUse = 0f;    
        ParticleEffects.Play();
    }
    public void Disable()
    {
        SetVisualStatus(false);
        _effectVolume.weight = 0;
    }
    public float GetFocusPercent() => _slowmoTime / _slowmoTimeMax;
    void SetTimeScaleValue(float value)
    {
        Time.timeScale = value;
    } 
}
