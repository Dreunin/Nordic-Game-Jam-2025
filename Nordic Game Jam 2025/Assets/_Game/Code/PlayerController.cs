using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerStats _stats;
    private Rigidbody _rb;
    private CapsuleCollider _col;

    public event Action Landed;
    public event Action Jumped;
    public event Action FelloffLedge;

    private float _jumpCooldown;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _col = GetComponent<CapsuleCollider>();
    }

    private Vector2 currentInput;
    
    private void Update()
    {
        _jumpCooldown += Time.deltaTime;
        currentInput = GatherInput();
    }

    private Vector2 GatherInput()
    {
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")); //Update to new input
        return input;
    }

    private void FixedUpdate()
    {
        HandleJump();
        HorizontalMovement();
    }

    private bool _grounded;

    private void HandleJump()
    {
        if (_grounded && _jumpCooldown > _stats.JumpCooldown)
        {
            //If pressing jump
            if (Input.GetButtonDown("Jump"))
            {
                Jump();
                _jumpCooldown = 0;
            }
        }
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
            _rb.AddForce(new Vector3(_stats.Acceleration * currentInput.x,0,0));
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

