using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    #region Object and Components

    public GameObject EMP;

    Rigidbody rb;
    BoxCollider groundCheck;
    Transform firePosition;

    #endregion

    bool isGrounBuffering = false;
    bool isDiving = false;

    public float GravityScale = 1f;

    public float Acceleration = 2.5f;
    public float MaxRunVelocity = 10f;

    public int NumberOfJumps = 1;
    [SerializeField] int currentJumps;
    public float JumpForce = 10f;
    public float DiveForce = 10f;

    public float FireForce = 5f;
    bool empOut = false;

    Vector3 lookDirection;

    private void Awake()
    {
        #region Get Components

        rb = GetComponent<Rigidbody>();

        #endregion
    }

    // Start is called before the first frame update
    void Start()
    {
        groundCheck = transform.Find("GroundCheck").GetComponent<BoxCollider>();
        firePosition = transform.Find("FirePosition");
        currentJumps = NumberOfJumps;
        InputManager.Instance.jumpInput.performed += PerformJump;
        InputManager.Instance.diveInput.performed += PerformDive;
        InputManager.Instance.fireInput.performed += PerformFire;

        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {


        lookDirection = new Vector3(Camera.main.transform.forward.x, 0, Camera.main.transform.forward.z);



    }

    private void FixedUpdate()
    {
        // GRAVITY

        rb.AddForce(Physics.gravity * GravityScale, ForceMode.Acceleration);


        // MATCH ROTATION TO CAMERA

        transform.rotation = Quaternion.LookRotation(lookDirection, Vector3.up);


        // MOVE THE PLAYER

        // TODO: make sure no more velocity is added when max run speed is reached
        if (true)
        {
            // get the input direction from the input manager
            Vector2 inputDirection = InputManager.Instance.moveInput.ReadValue<Vector2>();

            // convert input into the direction of applied force
            Vector3 force = transform.TransformDirection(new Vector3(inputDirection.x, 0f, inputDirection.y));

            //transform.forward

            // add force
            rb.AddForce(force.normalized * Acceleration * Time.deltaTime * 100);
        }


        // GROUND CHECK


        if(currentJumps != NumberOfJumps && !isGrounBuffering)
        {
            

            Collider[] foundColliders;


            foundColliders = Physics.OverlapBox(groundCheck.gameObject.transform.position, groundCheck.size / 2, groundCheck.transform.rotation, groundCheck.includeLayers);
            //Debug.Log($"Found {foundColliders.Length} colliders in ground check | Checking at center position {groundCheck.gameObject.transform.position}");

            if (foundColliders.Length > 0 )
            {
                

                
                //StartCoroutine(GroundBuffer());


            }
        }
        



    }

    void PerformJump(InputAction.CallbackContext context)
    {
        if (currentJumps > 0)
        {
            // this will cancel a dive, thus jumping out of it
            isDiving = false;

            currentJumps--;

            rb.AddForce(transform.up * JumpForce, ForceMode.Impulse);
        }
    }

    void PerformDive(InputAction.CallbackContext context)
    {
        if (!isDiving)
        {
            Debug.Log("Player Diving");

            isDiving = true;

            StartCoroutine(Dive());
        }

    }

    void PerformFire(InputAction.CallbackContext context)
    {
        if (empOut)
        {
            empOut = false;
        }
        else
        {
            GameObject firedEmp = Instantiate(EMP, Camera.main.transform.position + Camera.main.transform.forward, Quaternion.LookRotation(Camera.main.transform.forward));
            firedEmp.GetComponent<Rigidbody>().AddForce(Camera.main.transform.forward * FireForce * Time.deltaTime, ForceMode.Impulse);
        }
        
    }



    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Player Collision Hit");

        // GROUND CHECK

        Collider[] foundColliders;
        foundColliders = Physics.OverlapBox(groundCheck.center, groundCheck.size / 2, groundCheck.transform.rotation, groundCheck.includeLayers);
        if(foundColliders.Length > 0)
        {
            isDiving = false;

            StartCoroutine(GroundBuffer());
        }

        
        
    }

    // checks to make sure the player is grounded for a few frames before restoring jumps
    IEnumerator GroundBuffer()
    {
        isGrounBuffering = true;

        yield return new WaitForFixedUpdate();

        //Debug.Log("Starting buffer");
        int numOfFrames = 5;

        Collider[] foundColliders;

        bool restoreJumps = true;

        for (int i = 0; i < numOfFrames; i++)
        {
            foundColliders = Physics.OverlapBox(groundCheck.center, groundCheck.size / 2, groundCheck.transform.rotation, groundCheck.includeLayers);

            if (foundColliders.Length == 0)
            {
                Debug.Log("buffer Cancel");
                
                restoreJumps = false;

                break;
            }

            yield return new WaitForFixedUpdate();
        }

        if (restoreJumps)
        {
            currentJumps = NumberOfJumps;


        }

        isGrounBuffering = false;
    }

    IEnumerator Dive()
    {
        rb.velocity = Vector3.zero;

        while (isDiving)
        {
            rb.AddForce(Vector3.down * DiveForce, ForceMode.Acceleration);


            yield return new WaitForFixedUpdate();
        }

        

    }
}
