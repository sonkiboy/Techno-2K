using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.InputSystem;

public class EMPBehavior : MonoBehaviour
{
    [SerializeField] GameObject testExplosionGraphic;

    Rigidbody rb;
    SphereCollider explosionCollider;

    public float ExplosionForce = 10f;
    public float ExplosionFuse = 1f;
    public float ExplosionRange = 1f;
    [SerializeField]LayerMask ExplosionMask;

    bool isExploding = false;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        
        InputManager.Instance.fireInput.performed += PerformExplode;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("EMP hit obj");
        rb.isKinematic = true;

        
    }

    void PerformExplode(InputAction.CallbackContext context)
    {
        
            InputManager.Instance.fireInput.performed -= PerformExplode;



            StartCoroutine(Explode());
        
        
    }


    IEnumerator Explode()
    {
        

        // wait for the next fixed update since we are dealing with physics
        yield return new WaitForFixedUpdate();

        testExplosionGraphic.SetActive(true);

        // get all the objects that need to be exploded and apply force based on their distance to the center
        Collider[] foundColliders = Physics.OverlapSphere(transform.position,ExplosionRange,ExplosionMask);

        
        foreach (Collider collider in foundColliders)
        {

            
                Rigidbody rb = collider.GetComponent<Rigidbody>();

                if (rb != null)
                {
                    Vector3 direction = (transform.position - collider.transform.position);

                    Vector3 force = -direction.normalized * ExplosionForce / direction.magnitude;

                    //Debug.Log($"Force read as {force} and Direction as {direction}");

                    rb.AddForce(force, ForceMode.Impulse);
                }
            
            
        }

        // lets the explosion graphic linger before destroying obj
        yield return new WaitForSeconds(.4f);

        Destroy(this.gameObject);
    }

}
