using System;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = System.Random;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class PlayerController : MonoBehaviour
{
    private static readonly int Hang = Animator.StringToHash("Hang");
    private static readonly int Up = Animator.StringToHash("Up");
    private static readonly int Down = Animator.StringToHash("Down");
    private static readonly int Sneak = Animator.StringToHash("Sneak");
    private static readonly int Walk = Animator.StringToHash("Walk");
    private static readonly int Grounded = Animator.StringToHash("Grounded");
    private static readonly int Climb = Animator.StringToHash("Climb");
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

    private Animator animator;

    [SerializeField] private float flipTime = 1f;
    
    private float _jumpTimestamp;

    private bool _disableMaxSpeed = false;
    private float _disableMaxSpeedTime = 0f;

    
    enum ClimbDirection { Left, Right }
    private Climbable _attachedClimbable;
    private ClimbDirection _climbDirection;
    private Climbable _inFrontOfClimbable;
    private float climbableBottom, climbableTop;
    [SerializeField] private AudioClip[] playerSounds; //Jump, 

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
        animator = GetComponentInChildren<Animator>();
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
        
        //Dotween enlarge and visible player
        transform.localScale = Vector3.zero;
        GetComponent<SpriteRenderer>().DOFade(0, 0);
        GetComponent<SpriteRenderer>().DOFade(1, 2f);
        transform.DOScale(1, 2f);
    }

    private Vector2 currentInput;
    
    private void Update()
    {
        currentInput = GatherInput();
    }

    public Vector2 GatherInput()
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

        if (_grounded && _disableMaxSpeedTime < Time.time - 0.5)
        {
            _disableMaxSpeed = false;
        }


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

        HandleSprite();
    }

    [SerializeField] float upDownThreshold = 0.1f;
    
    private void HandleSprite()
    {
        //If x velocity is greater than 0, rotate sprite to the left
        if (_rb.linearVelocity.x < 0)
        {
            //Dotween flip the sprite
            transform.DOScaleX(1,Mathf.Abs(transform.localScale.x)*flipTime);
        }
        else if (_rb.linearVelocity.x > 0)
        {
            transform.DOScaleX(-1,Mathf.Abs(transform.localScale.x)*flipTime);
        }
        
        //If y velocity is greater than 0, play jump animation
        if (_rb.linearVelocity.y > upDownThreshold)
        {
            animator.SetBool(Up, true);
            animator.SetBool(Down, false);
            animator.SetBool(Hang, true);
        }
        else if (_rb.linearVelocity.y < -upDownThreshold)
        {
            animator.SetBool(Up, false);
            animator.SetBool(Down, true);
            animator.SetBool(Hang, true);
        } else if (_rb.linearVelocity.y == 0 && !_grounded)
        {
            animator.SetBool(Up, false);
            animator.SetBool(Down, false);
            animator.SetBool(Hang, true);
        }
        
        animator.SetBool(Grounded,_grounded);
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
        
        GetComponent<AudioSource>().PlayOneShot(playerSounds[UnityEngine.Random.Range(0,playerSounds.Length)]);
        
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
        float forceToAdd = _stats.Acceleration * currentInput.x * (!_grounded ? _stats.inAirMovementModifier : 1);
        if (Mathf.Abs(currentInput.x) < _stats.RunThreshold && currentInput.x > 0)
        {
            //Sneak
            forceToAdd /= 2;
            _rb.AddForce(new Vector3(forceToAdd,
                0,0));
            //Clamp to max speed/2 (half speed when sneaking)
            if (!_disableMaxSpeed)
            {
                _rb.linearVelocity = new Vector3(
                    Mathf.Clamp(_rb.linearVelocity.x, -_stats.MaxSpeed/2, _stats.MaxSpeed/2),_rb.linearVelocity.y,0);
            }
            animator.SetBool(Sneak, true);
            animator.SetBool(Walk,false);
        }
        else if (Mathf.Abs(currentInput.x) >= _stats.RunThreshold)
        {
            //Run
            _rb.AddForce(new Vector3(forceToAdd,
                0,0));
            //Clamp to max speed
            if (!_disableMaxSpeed)
            {
                _rb.linearVelocity = new Vector3(
                    Mathf.Clamp(_rb.linearVelocity.x, -_stats.MaxSpeed, _stats.MaxSpeed),_rb.linearVelocity.y,0);
            }
            animator.SetBool(Sneak, false);
            animator.SetBool(Walk,true);
        }
        else
        {
            animator.SetBool(Sneak, false);
            animator.SetBool(Walk,false);
        }
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
        //Dotween sprite
        transform.DOScaleX(direction == ClimbDirection.Left ? -1 : 1, 0.2f);
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

        if (other.CompareTag("Jumppad"))
        {
            Jumppad jumppad = other.transform.parent.GetComponent<Jumppad>();
            if (jumppad.used) return;
            float angle = jumppad.angle * Mathf.Deg2Rad;
            _rb.AddForce(new Vector3(Mathf.Sin(angle), Mathf.Cos(angle), 0) * jumppad.strength);
            _disableMaxSpeed = true;
            _disableMaxSpeedTime = Time.time;
            jumppad.Use();
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
        animator.SetBool(Climb,true);
    }

    private void DetachFromClimbable()
    {
        _attachedClimbable = null;
        _rb.useGravity = true;
        animator.SetBool(Climb, false);
    }
}

