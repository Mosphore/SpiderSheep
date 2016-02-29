using UnityEngine;
using System.Collections;



public class Car : MonoBehaviour
{

    public float moveSpeed = 6; // move speed
    public float turnSpeed = 90; // turning speed (degrees/second)
    public float lerpSpeed = 10; // smoothing speed
    public float gravity = 10; // gravity acceleration
    public bool isGrounded;
    public float deltaGround = 0.2f; // character is grounded up to this distance
    public float jumpSpeed = 10; // vertical jump initial speed
    public float jumpRange = 10; // range to detect target wall

    Vector3 surfaceNormal; // current surface normal
    Vector3 myNormal; // character normal
    float distGround; // distance from character position to ground
    bool jumping = false; // flag &quot;I'm jumping to wall&quot;
    float vertSpeed = 0; // vertical jump current speed 

    AudioSource EngineSound;

    void Start()
    {
        myNormal = transform.up; // normal starts as character up direction 
        GetComponent<Rigidbody>().freezeRotation = true; // disable physics rotation
                                         // distance from transform.position to ground
        distGround = GetComponent<BoxCollider>().bounds.extents.y - GetComponent<BoxCollider>().center.y;
        EngineSound = GetComponent<AudioSource>();
    }

    void FixedUpdate()
    {
        // apply constant weight force according to character normal:
        GetComponent<Rigidbody>().AddForce(-gravity * GetComponent<Rigidbody>().mass * myNormal);
    }

    void Update()
    {
        // jump code - jump to wall or simple jump
        if (jumping) return;  // abort Update while jumping to a wall
        Ray ray;
        RaycastHit hit;
        if (Input.GetButtonDown("Jump"))
        { // jump pressed:
            //ray = new Ray(transform.position, transform.forward);
            //if (Physics.Raycast(ray, out hit, jumpRange))
            //{ // wall ahead?
            //    StartCoroutine(JumpToWall(hit.point, hit.normal)); // yes: jump to the wall
            //}
            if (isGrounded)
            { // no: if grounded, jump up
                GetComponent<Rigidbody>().velocity += jumpSpeed * myNormal;
            }
        }

        // movement code - turn left/right with Horizontal axis:
        transform.Rotate(0, Input.GetAxis("Horizontal") * turnSpeed * Time.deltaTime, 0);
        // update surface normal and isGrounded:
        ray = new Ray(transform.position, -myNormal); // cast ray downwards
        if (Physics.Raycast(ray, out hit,30))
        { // use it to update myNormal and isGrounded
          
            isGrounded = hit.distance <= distGround + deltaGround;
            surfaceNormal = hit.normal;

        }
        else {
            isGrounded = false;
            // assume usual ground normal to avoid "falling forever"
            surfaceNormal = Vector3.up;
        }
        myNormal = Vector3.Lerp(myNormal, surfaceNormal, lerpSpeed * Time.deltaTime);
        // find forward direction with new myNormal:
        var myForward = Vector3.Cross(transform.right, myNormal);
        // align character to the new myNormal while keeping the forward direction:
        var targetRot = Quaternion.LookRotation(myForward, myNormal);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, lerpSpeed * Time.deltaTime);
        // move the character forth/back with Vertical axis:
        GetComponent<Rigidbody>().AddRelativeForce(0,0,Input.GetAxis("Vertical") * moveSpeed) ;
        //transform.Translate(0, 0, Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime);
    }

    IEnumerator JumpToWall(Vector3 point, Vector3 normal)
    {
        // jump to wall 
        jumping = true; // signal it's jumping to wall
        GetComponent<Rigidbody>().isKinematic = true; // disable physics while jumping
        var orgPos = transform.position;
        var orgRot = transform.rotation;
        var dstPos = point + normal * (distGround + 0.5f); // will jump to 0.5 above wall
        var myForward = Vector3.Cross(transform.right, normal);
        var dstRot = Quaternion.LookRotation(myForward, normal);
        for (float t = 0.0f; t < 1.0f; )
        {
            t += Time.deltaTime;
            transform.position = Vector3.Lerp(orgPos, dstPos, t);
            transform.rotation = Quaternion.Slerp(orgRot, dstRot, t);
            yield return 0; // return here next frame
        }
        myNormal = normal; // update myNormal
        GetComponent<Rigidbody>().isKinematic = false; // enable physics
        jumping = false; // jumping to wall finished
    }
}






