using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    public Rigidbody rb;

    public float forwardAcceleration = 8f, reverseAcceleration = 4f, maxSpeed = 50f, turnStrength = 180f;

    public float gravityForce = 10f;
    public float dragOnGround = 3f;

    private bool grounded;

    public LayerMask whatIsGround;
    public float groundRayLength = 0.5f;
    public Transform groundRayPoint;

    private float speedInput, turnInput;

    public Transform leftFrontWheel, rightFrontWhell;

    public float maxWheelTurn = 25f;

    public ParticleSystem[] dustTrail;
    public float maxEmissionValue = 25f;
    private float emissionRate;

    // Start is called before the first frame update
    void Start()
    {
        rb.transform.parent = null;
    }

    // Update is called once per frame
    void Update()
    {
        speedInput = 0;

        if(Input.GetAxis("Vertical") > 0){
            speedInput = Input.GetAxis("Vertical") * forwardAcceleration * 1000f;
        } else if (Input.GetAxis("Vertical") < 0)
        {
            speedInput = Input.GetAxis("Vertical") * reverseAcceleration * 1000f;
        }

        turnInput = Input.GetAxis("Horizontal");
        
        if(grounded){
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles 
        + new Vector3(0f, turnInput * turnStrength * Time.deltaTime * Input.GetAxis("Vertical"), 0f));
        }

        leftFrontWheel.localRotation = Quaternion.Euler
        (leftFrontWheel.localRotation.eulerAngles.x, turnInput * maxWheelTurn - 180, leftFrontWheel.localRotation.eulerAngles.z);

        rightFrontWhell.localRotation = Quaternion.Euler
        (rightFrontWhell.localRotation.eulerAngles.x, turnInput * maxWheelTurn, rightFrontWhell.localRotation.eulerAngles.z);


        transform.position = rb.transform.position;
    }

    private void FixedUpdate() {
        grounded = false;

        RaycastHit hit;

        if(Physics.Raycast(groundRayPoint.position, -transform.up, out hit, groundRayLength, whatIsGround)){
            grounded = true;

            transform.rotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
        }

        emissionRate = 0;


        if(Mathf.Abs(speedInput) > 0 && grounded){

            rb.drag = dragOnGround;
            rb.AddForce(transform.forward * speedInput);
            emissionRate = maxEmissionValue;

        } else {
            rb.drag = 0.1f;
            rb.AddForce(Vector3.up * -gravityForce * 100f);
        }

        foreach(ParticleSystem part in dustTrail){
            var emissionModule = part.emission;
            emissionModule.rateOverTime = emissionRate;
        }
    }

}


// https://www.youtube.com/watch?v=cqATTzJmFDY