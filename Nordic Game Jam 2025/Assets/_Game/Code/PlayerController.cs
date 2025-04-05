using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerStats _stats;
    private Rigidbody _rb;
    private CapsuleCollider _col;
    private PlayerInput _input;
    
    private InputActionMap _inputMap;
    private InputAction _move;
    private InputAction _jump;

    private float groundedRaycastLength = 0.1f;

    public event Action Landed;
    public event Action Jumped;
    public event Action FelloffLedge;

    private float _jumpTimestamp;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _col = GetComponent<CapsuleCollider>();
        _input = GetComponent<PlayerInput>();
        
        DontDestroyOnLoad(gameObject);
    }
    
    public void ResetVelocity()
    {
        _rb.linearVelocity = Vector3.zero;
    }

    private void Start()
    {
        _inputMap = _input.actions.FindActionMap("Infiltration");
        _move = _inputMap.FindAction("Move");
        _jump = _inputMap.FindAction("Jump");
        _jump.performed += HandleJump;
    }

    private Vector2 currentInput;
    
    private void Update()
    {
        currentInput = GatherInput();
    }

    private Vector2 GatherInput()
    {
        Vector2 input = _move.ReadValue<Vector2>();
        return input;
    }

    private void FixedUpdate()
    {
        //Raycast  down to check if grounded
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, _col.bounds.extents.y + groundedRaycastLength))
        {
            _grounded = true;
        }
        else
        {
            _grounded = false;
        }
        
        HorizontalMovement();
    }

    private bool _grounded;

    private void HandleJump(InputAction.CallbackContext ctx)
    {
        if (!_grounded || _jumpTimestamp + _stats.JumpCooldown > Time.time) return;
        
        Jump();
        _jumpTimestamp = Time.time;
    }

    private void Jump()
    {
        //Jump
        float jumpVelocity = _stats.JumpPower;
        
        //Apply force
        _rb.AddForce(new Vector3(0,jumpVelocity,0));
        
        Jumped?.Invoke();
    }

    private void HorizontalMovement()
    {
        if (currentInput.x == 0)
        {
            //Deaccelerate
        }
        else
        {
            _rb.AddForce(new Vector3(_stats.Acceleration * currentInput.x * (!_grounded ? _stats.inAirMovementModifier : 1)
                ,0,0));
            //Clamp to max speed
            _rb.linearVelocity = new Vector3(
                Mathf.Clamp(_rb.linearVelocity.x, -_stats.MaxSpeed, _stats.MaxSpeed),_rb.linearVelocity.y,0);
            
            /*if (currentInput.x < _stats.RunThreshold)
            {
                //Sneak
            }
            else
            {
                //Run
            }*/
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        _grounded = true;
    }

    private void OnCollisionExit(Collision other)
    {
        _grounded = false;
    }
}

