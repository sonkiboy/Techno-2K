using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    public PlayerInputs playerControls;

    public InputAction moveInput;
    public InputAction lookInput;
    public InputAction fireInput;
    public InputAction jumpInput;
    public InputAction diveInput;

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    private void Awake()
    {
        // if there is no saved instance in the static variable, save this instance
        if(Instance == null)
        {
            Instance = this;

            playerControls = new PlayerInputs();

            moveInput = playerControls.Player.Move;
            lookInput = playerControls.Player.Look;
            fireInput = playerControls.Player.Fire;
            jumpInput = playerControls.Player.Jump;
            diveInput = playerControls.Player.Dive;

        }
        // if there is a saved instance already, delete this duplicate 
        else
        {
            Destroy(this);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
