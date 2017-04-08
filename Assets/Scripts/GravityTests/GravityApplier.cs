using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityApplier : MonoBehaviour
{
    public Rigidbody rigidbody = null;
    public GameObject hitIndicator = null;

    // Use this for initialization
    void Start ()
    {
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.useGravity = false;
    }

    private bool rotate = false;
    private bool up = false;
    private bool down = false;
    private bool left = false;
    private bool right = false;
    private bool jump = false;
    private RaycastHit latestHit;
    private Vector3 gravityUp = new Vector3(0, -1, 0);

    // Update is called once per frame
    void Update ()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            rotate = true;
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            down = true;
        }
        else
            down = false;

        if (Input.GetKey(KeyCode.UpArrow))
        {
            up = true;
        }
        else
            up = false;

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            left = true;
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            right = true;
        }

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            jump = true;
        }

        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 10000.0f))
            {
                GameObject indicator = GameObject.Instantiate(hitIndicator, hit.point, Quaternion.identity );
                Destroy(indicator.GetComponent<Rigidbody>());

                rotate = true;
                latestHit = hit;

                gravityUp = hit.normal.normalized;

                Debug.Log("You selected the " + hit.transform.name + " // " + gravityUp); // ensure you picked right object
            }
        }
    }

    private void FixedUpdate()
    {
        //GetComponent<Rigidbody>().AddRelativeForce( new Vector3(0, -2.81f, 0) );
        float gravitySize = -9.81f;
        //Debug.Log(gravitySize + " // " + transform.up);

        if ( rotate )
        {
            //GetComponent<Rigidbody>().velocity = Vector3.zero;
            //Vector3 angles = this.transform.eulerAngles;
            //angles.x += 90;
            //this.transform.eulerAngles = angles;



            //this.transform.Rotate(new Vector3(0, 0, -90), Space.Self);

            //this.transform.rotation.

            Debug.Log(this.transform.eulerAngles);

            //GetComponent<Rigidbody>().AddForce(gravitySize * transform.up * 2, ForceMode.VelocityChange);

            Debug.LogError("SWITCHED GRAVITY DIRECTION!");

            rotate = false;
        }


        //GetComponent<Rigidbody>().AddForce(gravitySize * transform.up, ForceMode.Acceleration);
        GetComponent<Rigidbody>().AddForce(gravitySize * gravityUp, ForceMode.Acceleration);

        if (up)
        {
            if( Mathf.Abs(rigidbody.velocity.x) < 30 )
                GetComponent<Rigidbody>().AddRelativeForce(Vector3.forward * 20, ForceMode.Acceleration);
            //rigidbody.angularVelocity = Vector3.zero;
            //rigidbody.velocity = rigidbody.velocity + (-Vector3.right * 30 * Time.fixedDeltaTime);
        }
        if (down) 
        {
            if ( Mathf.Abs(rigidbody.velocity.x) < 30)
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
            GetComponent<Rigidbody>().AddRelativeForce(Vector3.up * 10, ForceMode.VelocityChange);
            jump = false;
        }

        
        Debug.Log(rigidbody.velocity + " // " + this.transform.right + " // " + this.transform.TransformDirection(this.transform.right) + " // " + projection + "// " + " // " + this.transform.InverseTransformDirection(projection) + " // " + projection.magnitude);

        //projection.x 

        //Debug.Log(rigidbody.velocity);
    }


}
