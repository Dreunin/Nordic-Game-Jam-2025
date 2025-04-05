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
    public event Action ClimbStarted;
    public event Action ClimbEnded;

    private float _jumpTimestamp;

    
    enum ClimbDirection { Left, Right }
    private Climbable _attachedClimbable;
    private ClimbDirection _climbDirection;
    private Climbable _inFrontOfClimbable;
    private float climbableBottom, climbableTop;

    private void Awake()
    {
        if(GameObject.FindGameObjectWithTag("Player") != this.gameObject)
        {
            Destroy(gameObject);
            return;
        }
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
        //Physics.Raycast(new Vector3(_col.bounds.center.x, _col.bounds.min.y), Vector3.down, out hit, groundedRaycastLength) ||
        //Physics.Raycast(new Vector3(_col.bounds.max.x, _col.bounds.min.y), Vector3.down, out hit, groundedRaycastLength))
        _grounded = Physics.Raycast(new Vector3(_col.bounds.min.x, transform.position.y), Vector3.down, out hit,
                        _col.bounds.extents.y + groundedRaycastLength) ||
                    Physics.Raycast(new Vector3(_col.bounds.center.x, transform.position.y), Vector3.down, out hit,
                        _col.bounds.extents.y + groundedRaycastLength) ||
                    Physics.Raycast(new Vector3(_col.bounds.max.x, transform.position.y), Vector3.down, out hit,
                        _col.bounds.extents.y + groundedRaycastLength);
                    ;

        if (_rb.linearVelocity.y < -0.1 && _inFrontOfClimbable is not null)
        {
            AttachToClimbable(_inFrontOfClimbable);
        }

        if (_attachedClimbable is null)
        {
            HorizontalMovement();
        }
        else
        {
            ClimbingMovement();
        }
    }

    private void LateUpdate()
    {
        // Ensure the player cannot exceed the bound of climbable
        if (_attachedClimbable is not null)
        {
            if (_col.bounds.min.y < climbableBottom)
            {
                _rb.position = new Vector3(_rb.position.x, _rb.position.y + climbableBottom - _col.bounds.min.y, _rb.position.z);
            } else if (_col.bounds.max.y > climbableTop)
            {
                _rb.position = new Vector3(_rb.position.x, _rb.position.y + climbableTop - _col.bounds.max.y, _rb.position.z);
            }
        }
    }

    private bool _grounded;

    private void HandleJump(InputAction.CallbackContext ctx)
    {
        if (_jumpTimestamp + _stats.JumpCooldown > Time.time) return;

        if (_attachedClimbable is not null)
        {
            JumpOffClimbable();
        } else if (_grounded)
        {
            Jump();
        }
        
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

    // When jumping off a climbable, jump at an angle based on the left stick
    // If not using controller, just do a fixed angle
    private void JumpOffClimbable()
    {
        DetachFromClimbable();
        
        // Jump angle
        if (_input.currentControlScheme == "Gamepad" && currentInput.magnitude > 0.1f)
        {
            _rb.AddForce(currentInput.normalized * _stats.JumpPower);
        }
        else
        {
            float jumpAngle = Mathf.PI / 3f * (_climbDirection == ClimbDirection.Right ? 1 : -1);
            _rb.AddForce(new Vector2(Mathf.Sin(jumpAngle), Mathf.Cos(jumpAngle)) * _stats.JumpPower);
        }
    }

    private void HorizontalMovement()
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

    private void ClimbingMovement()
    {
        float velocity = currentInput.y * _stats.ClimbSpeed;
        _rb.linearVelocity = new Vector3(0, velocity);
        
        // Switch sides
        if (_climbDirection == ClimbDirection.Left && currentInput.x > 0.5)
        {
            SwitchClimbingSide(ClimbDirection.Right);
        } else if (_climbDirection == ClimbDirection.Right && currentInput.x < -0.5)
        {
            SwitchClimbingSide(ClimbDirection.Left);
        }
    }

    private void SwitchClimbingSide(ClimbDirection direction)
    {
        _climbDirection = direction;
        float newPosition = _attachedClimbable.transform.position.x + (direction == ClimbDirection.Left ? -1 : 1) * _attachedClimbable.offset;
        transform.position = new Vector3(newPosition, transform.position.y, transform.position.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Handle attaching to climbable objects
        if (other.CompareTag("Climbable"))
        {
            if (!_grounded)
            {
                AttachToClimbable(other.GetComponent<Climbable>());
            }
            else
            {
                _inFrontOfClimbable = other.GetComponent<Climbable>();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Climbable"))
        {
            _inFrontOfClimbable = null;
        }
    }

    private void AttachToClimbable(Climbable climbable)
    {
        _rb.linearVelocity = Vector3.zero;
        _attachedClimbable = climbable;
        _rb.useGravity = false;
        _inFrontOfClimbable = null;
        SwitchClimbingSide(_rb.linearVelocity.x > 0 ? ClimbDirection.Right : ClimbDirection.Left);

        Collider collider = _attachedClimbable.GetComponent<Collider>(); 
        climbableBottom = collider.bounds.min.y;
        climbableTop = collider.bounds.max.y;
    }

    private void DetachFromClimbable()
    {
        _attachedClimbable = null;
        _rb.useGravity = true;
    }
}

