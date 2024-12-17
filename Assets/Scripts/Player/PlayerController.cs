using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    private Rigidbody2D _rb;
    private Keybinds _keybinds;
    private Vector2 _wishDir = Vector2.zero;
    [SerializeField] private bool _jumpRequest = false;
    [SerializeField] Vector2 _playerSize = Vector2.one;

    [SerializeField] float _groundCheckDistance = 0.1f;
    [SerializeField] float _rideHeight = 0.5f;
    [SerializeField] float _rideHeightBuffer = 0.6f;
    [SerializeField] float _rideSpringStrength = 10;
    [SerializeField] float _rideSpringDamping = 1;    
    [SerializeField] float _jumpForce = 150;
    [SerializeField] float _maxJumpTime = 0.125f;
    [SerializeField] LayerMask _groundMask;
    [SerializeField] private bool _inJump = false;
    [SerializeField] private float _timeInJump = 0;
    [SerializeField] private int _jumpCount = 0;
    [SerializeField] int _maxJumpCount = 2;
    [SerializeField] private bool _jumpCancelRequest = false;
    [SerializeField] private bool _jumped = false;
    
    [SerializeField] private float _topSpeed;
    [SerializeField] private float _acceleration;
    [SerializeField] private float _airAcceleration;
    [SerializeField] private float _gravityScale = 3;    
    [SerializeField] private TimeJump _jumpDrive;
    public Action<float> UpdateControlAuthority;
    private float _controlAuthority = 1;

    public Interaction CurrentInteraction = null;
    
    void Awake()
    {
        _keybinds = new();
    }
   
    void OnEnable()
    {
        _keybinds.Enable();
        LinkKeybinds();
    }
    void OnDisable()
    {
        _keybinds.Disable();
        UnlinkKeybinds();
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.layer != LayerMask.NameToLayer("DeathTrigger"))
            return;
        GameManager.Instance.PlayerDied();
    }

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _rb.gravityScale = _gravityScale;
        UpdateControlAuthority += SetControlAuthority;
    }

    /// <summary>
    /// Update is called once per frame 
    /// </summary>
    void Update()
    {
        IncrementTimers();
        ProcessJump();
    }
    
    /// <summary>
    /// Called every physics update
    /// </summary>
    void FixedUpdate()
    {
        ApplyJumpForce();
        ApplyMoveForce();

        ApplyHoverForce();
        if(IsGrounded())
            _jumpDrive.ResetJumpDrive();
    }
    void SetControlAuthority(float authority)
    {
        
        // _rb.gravityScale = _gravityScale * authority;
        _controlAuthority = authority;

        // if (authority != 1)
        // {
        //     _rb.gravityScale = 0;
        //     _controlAuthority = 0.01f;
        // }
        // else
        // {
        //     _rb.gravityScale = _gravityScale;
        //     _controlAuthority = 1;
        // }
    }
    void ApplyMoveForce()
    {
        if(IsGrounded())
        {
            float targetSpeed = _wishDir.x * _topSpeed;
            float diff = targetSpeed - _rb.linearVelocityX;

            float direction = Mathf.Sign(diff);
            float magnitude = Mathf.Clamp(Mathf.Abs(diff), 0, _acceleration * Time.fixedDeltaTime);
            float velocityDiff = direction * magnitude * _controlAuthority;
            _rb.linearVelocityX += velocityDiff;
        }
        else
        {
            _rb.linearVelocityX += _airAcceleration * Time.fixedDeltaTime * _wishDir.x;
            _rb.linearVelocityX = Mathf.Clamp(_rb.linearVelocityX, -_topSpeed, _topSpeed);
        }
    }
    public void DieHorribleDeath()
    {
        _jumpDrive.DisableSlowmotion();
    }
    void ProcessJump()
    {
        if(IsGrounded() && _inJump && _timeInJump > 0.25f)
        {
            _jumpCount = 0;
            _inJump = false;
        }

        if (_jumpRequest && CanJump() && !_jumped)
        {
            _inJump = true;
            _jumped = true;
            _timeInJump = 0;
            if ( _rb.linearVelocity.y < 0)
                _rb.linearVelocity = new(_rb.linearVelocity.x, 0);
        }

        if (_inJump && _jumpCancelRequest && _rb.linearVelocity.y > 0)
            _rb.linearVelocity = new(_rb.linearVelocity.x, 0);
        _jumpCancelRequest = false;  
    }

    void ApplyJumpForce()
    {
        if (_inJump && _jumpRequest && _timeInJump < _maxJumpTime)
        {
            _rb.AddForce(Vector2.up * _jumpForce * Time.fixedDeltaTime);
        }
    }
    bool CanJump()
    {
        return IsGrounded() || _jumpCount <= _maxJumpCount;
    }

    bool IsGrounded()
    {
        RaycastHit2D hit = Physics2D.BoxCast(transform.position, _playerSize, 0, Vector2.down, _groundCheckDistance, _groundMask);
        return hit;
    }

    void ApplyHoverForce()
    {
        if (_inJump || _timeInJump < 0.25f || _controlAuthority < 1)
            return;

        RaycastHit2D hit = Physics2D.BoxCast(transform.position, _playerSize, 0, Vector2.down, _rideHeightBuffer, _groundMask);
        if(!hit)
            return;

        float velocity = Vector2.Dot(Vector2.down, _rb.linearVelocity);
        float delta = hit.distance - _rideHeight;
        float force = (delta * _rideSpringStrength) - (velocity * _rideSpringDamping);

        _rb.AddForce(force * Time.fixedDeltaTime * Vector2.down);
    }

    void IncrementTimers()
    {
        float delta = Time.deltaTime;
        _timeInJump += delta;
    }

#region Keybinds
    void OnMovePerformed(InputAction.CallbackContext ctx)
    {
        _wishDir = ctx.ReadValue<Vector2>();
    }
    void OnMoveCanceled(InputAction.CallbackContext ctx)
    {
        _wishDir = Vector2.zero;
    }

    void OnJumpPerformed(InputAction.CallbackContext ctx)
    {
        _jumpCount++;
        _jumpRequest = true;
    }
    
    void OnJumpCanceled(InputAction.CallbackContext ctx)
    {
        _jumpRequest = false;
        _jumpCancelRequest = true;
        _jumped = false;
    }

    void OnInteractionPerformed(InputAction.CallbackContext ctx)
    {
        Debug.Log("Interaction performed");
        CurrentInteraction?.Interact();
    }

    /// <summary>
    /// Link all methods to inputactions object
    /// </summary>
    void LinkKeybinds()
    {
        _keybinds.Player.Move.performed += OnMovePerformed;
        _keybinds.Player.Move.canceled += OnMoveCanceled;
        _keybinds.Player.Jump.performed += OnJumpPerformed;
        _keybinds.Player.Jump.canceled += OnJumpCanceled;
        _keybinds.Player.Crouch.performed += _jumpDrive.EnableSlowmotion;
        _keybinds.Player.Crouch.canceled += _jumpDrive.SlowmotionProxy;
        _keybinds.Player.Sprint.performed += _jumpDrive.PerformTimeLeap;
        _keybinds.Player.Interact.performed += OnInteractionPerformed;
    }

    /// <summary>
    /// Unlink all methods from inputactions object to avoid memory leaks
    /// </summary>
    void UnlinkKeybinds()
    {
        _keybinds.Player.Move.performed -= OnMovePerformed;
        _keybinds.Player.Move.canceled -= OnMoveCanceled;
        _keybinds.Player.Jump.performed -= OnJumpPerformed;
        _keybinds.Player.Jump.canceled -= OnJumpCanceled;
        _keybinds.Player.Crouch.performed -= _jumpDrive.EnableSlowmotion;
        _keybinds.Player.Crouch.canceled -= _jumpDrive.SlowmotionProxy;
        _keybinds.Player.Sprint.performed -= _jumpDrive.PerformTimeLeap;
        _keybinds.Player.Interact.performed -= OnInteractionPerformed;
    }
#endregion
}
