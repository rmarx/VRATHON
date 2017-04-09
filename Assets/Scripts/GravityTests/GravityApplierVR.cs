using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityApplierVR : MonoBehaviour
{
    public Rigidbody rigidbody = null;
    public GameObject hitIndicator = null;

    // Use this for initialization
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.useGravity = false;

        immediateTransformer = (new GameObject()).transform;
    }

    private bool rotate = false;
    private bool up = false;
    private bool down = false;
    private bool left = false;
    private bool right = false;
    private bool jump = false;
    private RaycastHit latestHit;
    private Vector3 gravityUp = new Vector3(0, 1, 0);

    private bool hasTargetRotation = false;
    private Quaternion targetRotation;

    private Transform immediateTransformer;

    // Update is called once per frame
    void Update()
    {
        /*
        if (Input.GetKeyDown(KeyCode.Space))
        {
            rotate = true;
        }

        */

        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
        {
            down = true;
        }
        else
            down = false;

        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
        {
            up = true;
        }
        else
            up = false;

        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            left = true;
        }

        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            right = true;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            jump = true;
        }

        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 10000.0f))
            {
                //GameObject indicator = GameObject.Instantiate(hitIndicator, hit.point, Quaternion.identity );
                //Destroy(indicator.GetComponent<Rigidbody>());

                if (Vector3.Dot(hit.normal, -this.transform.forward) > 0.7f)
                {
                    Debug.LogError("hit a plane that is facing towards us, so we're not switching to that one!");
                }
                else
                {

                    rotate = true;
                    latestHit = hit;

                    gravityUp = hit.normal.normalized;

                    Debug.Log("You selected the " + hit.transform.name + " // " + gravityUp); // ensure you picked right object
                }
            }
        }

        if (hasTargetRotation)
        {
            float step = 500 * Time.deltaTime;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, step);
        }
    }

    private void FixedUpdate()
    {
        //GetComponent<Rigidbody>().AddRelativeForce( new Vector3(0, -2.81f, 0) );
        float gravitySize = -9.81f;
        //Debug.Log(gravitySize + " // " + transform.up);

        if (rotate)
        {
            //GetComponent<Rigidbody>().velocity = Vector3.zero;
            //Vector3 angles = this.transform.eulerAngles;
            //angles.x += 90;
            //this.transform.eulerAngles = angles;

            if (gravityUp == this.transform.up)
            {
                // user lashed himself to the current running surface. This doesn't react as the others do, but initiates a jump!
                jump = true;
                Debug.LogWarning("Lashed to the current running surface : JUMPING!");
            }
            else
            {

                //this.transform.Rotate(new Vector3(0, 0, -90), Space.Self);

                //this.transform.rotation.

                Debug.Log(this.transform.eulerAngles);

                //GetComponent<Rigidbody>().AddForce(gravitySize * transform.up * 2, ForceMode.VelocityChange);


                Debug.LogError("SWITCHED GRAVITY DIRECTION! " + this.transform.forward + " // " + gravityUp + " // " + this.transform.up);

                //this.transform.rotation = Quaternion.LookRotation(this.transform.forward, gravityUp);
                targetRotation = Quaternion.LookRotation(this.transform.forward, gravityUp);
                hasTargetRotation = true;

                immediateTransformer.rotation = targetRotation; // we need transform.right to calculate extra impulse. We COULD do this by calculating the "new" transform.right ourselves, but this is easier right now :)

                Debug.LogError("SWITCHED GRAVITY DIRECTION! 2 " + this.transform.forward + " // " + gravityUp);

                //Vector3 currentForward = this.transform.forward;
                // this.transform.rotation = Quaternion.FromToRotation(this.transform.up, gravityUp);
                //this.transform.rotation = Quaternion.FromToRotation(this.transform.forward, currentForward);

                Vector3 impulseDirection = latestHit.point - this.transform.position;

                // we want to overcompensate to the left/right to get a better feel
                // so we project the impulse on our right axis and add half that to the final impulsedirection 
                Vector3 projection2 = Vector3.Project(impulseDirection.normalized, immediateTransformer.right);

                Debug.Log(projection2 + " // " + impulseDirection + " // " + impulseDirection.normalized + "//" + (impulseDirection.normalized + projection2 * 10));

                impulseDirection = impulseDirection.normalized + projection2 * 7;


                GetComponent<Rigidbody>().AddForce(impulseDirection * 5, ForceMode.VelocityChange);


                // https://forum.unity3d.com/threads/using-vector3-dot-for-direction-facing-calculation.173707/
                // if 2 vectors have opposite direction, dotproduct is -1
                // here, we want to know if we've gone from floor to ceiling
                // if that is the case, we want to add a little extra gravity push because otherwhise the falling would be too long 
                float opposites = Vector3.Dot(gravityUp, this.transform.up);
                Debug.LogError("OPPOSITES " + opposites);
                if (opposites < -0.7f)
                {
                    GetComponent<Rigidbody>().AddForce(-gravityUp * 5, ForceMode.VelocityChange);
                }
            }

            rotate = false;
        }


        //GetComponent<Rigidbody>().AddForce(gravitySize * transform.up, ForceMode.Acceleration);
        GetComponent<Rigidbody>().AddForce(gravitySize * gravityUp, ForceMode.Acceleration);

        if (up)
        {
            if (Mathf.Abs(rigidbody.velocity.x) < 30)
                GetComponent<Rigidbody>().AddRelativeForce(Vector3.forward * 20, ForceMode.Acceleration);
            //rigidbody.angularVelocity = Vector3.zero;
            //rigidbody.velocity = rigidbody.velocity + (-Vector3.right * 30 * Time.fixedDeltaTime);
        }
        if (down)
        {
            if (Mathf.Abs(rigidbody.velocity.x) < 30)
                GetComponent<Rigidbody>().AddRelativeForce(-Vector3.forward * 20, ForceMode.Acceleration);
            //rigidbody.angularVelocity = Vector3.zero;
            //rigidbody.velocity = rigidbody.velocity + (Vector3.right * 30 * Time.fixedDeltaTime);
        }

        // if we go left or right, we want to do a very snappy movement, i.e. negating all momentum in the other direction
        // we can do this by projecting the velocity to the left or right "downwards" onto the local transform.right Vector and then adding that to the force on the other side to "overcompensate" and effectively negate the other momentum

        // rigidbody.velocity and transform.right are both in GLOBAL space
        // we calculate in local because it's easier to reason about and left/right is always on the x-axis 
        Vector3 projection = Vector3.Project(rigidbody.velocity, this.transform.right);
        Vector3 projectionLocal = transform.InverseTransformDirection(projection);

        float sideSpeed = 5;

        if (left)
        {
            Vector3 direction = -Vector3.right * sideSpeed;
            if (projectionLocal.x > 1.0f) // means we're currently going right, we need to negate this
            {
                direction = new Vector3(projectionLocal.x * -1.0f, 0, 0);
            }
            GetComponent<Rigidbody>().AddRelativeForce(direction, ForceMode.VelocityChange);
            left = false;

            //if (Mathf.Abs(rigidbody.velocity.z) < 10)
            //    GetComponent<Rigidbody>().AddRelativeForce(-Vector3.right * 20, ForceMode.Acceleration);
        }
        if (right)
        {
            Vector3 direction = Vector3.right * sideSpeed;
            if (projectionLocal.x < -1.0f) // means we're currently going left, we need to negate this
            {
                direction = new Vector3(projectionLocal.x * -1.0f, 0, 0);
            }

            GetComponent<Rigidbody>().AddRelativeForce(direction, ForceMode.VelocityChange);
            right = false;
            //if (Mathf.Abs(rigidbody.velocity.z) < 10)
            //    GetComponent<Rigidbody>().AddRelativeForce(Vector3.right * 20, ForceMode.Acceleration); 
        }

        if (jump)
        {
            GetComponent<Rigidbody>().AddForce(immediateTransformer.up * 10, ForceMode.VelocityChange);
            jump = false;
        }


        //Debug.Log(rigidbody.velocity + " // " + this.transform.right + " // " + this.transform.TransformDirection(this.transform.right) + " // " + projection + "// " + " // " + this.transform.InverseTransformDirection(projection) + " // " + projection.magnitude);

        //projection.x 

        //Debug.Log(rigidbody.velocity);
    }


}
