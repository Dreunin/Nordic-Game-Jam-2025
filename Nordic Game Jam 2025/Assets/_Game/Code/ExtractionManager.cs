using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class ExtractionManager : MonoBehaviour
{
    // TODO:
    // Move crosshair
    // On tooth click: Break it
    // On non-tooth click: Lose
    // When all teeth are collected: Win

    [SerializeField] private GameObject crosshair;
    [SerializeField] private List<Tooth> _teeth;
    [SerializeField] private Animator _playerAnimator;
    private Collider2D _crosshairCollider;

    [SerializeField] private float crosshairSpeed;

    private PlayerInput _input;
    private InputActionMap _inputActionMap;
    private InputAction _move;
    private InputAction _extract;

    private bool won;
    
    public void Awake()
    {
        _crosshairCollider = crosshair.GetComponent<Collider2D>();

        _input = GetComponent<PlayerInput>();
        _inputActionMap = _input.actions.FindActionMap("Extraction");
        _move = _inputActionMap.FindAction("Move");
        _extract = _inputActionMap.FindAction("Extract");

        _extract.performed += OnExtract;
        _input.onControlsChanged += OnControlsChanged;
        
        OnControlsChanged(_input);
        
    }

    private void Update()
    {
        if (_input.currentControlScheme == "Gamepad")
        {
            crosshair.transform.position += (Vector3) _move.ReadValue<Vector2>() * (crosshairSpeed * Time.deltaTime);
        }
    }

    private void OnControlsChanged(PlayerInput input)
    {
        if (_input.currentControlScheme == "Gamepad")
        {
            crosshair.gameObject.SetActive(true);            
        } else if (_input.currentControlScheme == "Keyboard&Mouse")
        {
            crosshair.gameObject.SetActive(false);            
        }
    }

    private void OnExtract(InputAction.CallbackContext ctx)
    {
        List<Collider2D> overlap = new List<Collider2D>();
        bool toothFound = false;
        _playerAnimator.SetTrigger("Attack");
        
        if (_input.currentControlScheme == "Gamepad")
        {
            Physics2D.OverlapCollider(_crosshairCollider, overlap);
        } else if (_input.currentControlScheme == "Keyboard&Mouse")
        {
            Vector2 pos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            /*RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero);
            if (hit)
            {
                Debug.Log(hit);
                overlap.Add(hit.collider);
            }
            
            Collider2D collider = Physics2D.OverlapPoint(pos);
            if (collider is not null)
            {
                Debug.Log(collider.gameObject.name);
                overlap.Add(collider);
            }*/
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit2D hit = Physics2D.GetRayIntersection(ray);
            if (hit)
            {
                overlap.Add(hit.collider);
            }
        }
        
        foreach (Collider2D other in overlap)
        {
            Tooth tooth = other.GetComponent<Tooth>();
            if (tooth is null) continue;
            
            tooth.Collect();
            toothFound = true;
        }

        if (!toothFound) Lose();
        if (_teeth.All(t => t.IsCollected()) && !won)
        {
            won = true;
            Invoke(nameof(Win),2f);
        }
    }

    public GameObject canvas;
    
    private void Win()
    {
        canvas.SetActive(true);
        Debug.Log("Win");
    }

    private void Lose()
    {
        Debug.Log("Lose");
    }
    
}
