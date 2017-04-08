using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    public OompaLoompa orange = null;
    
    private void Awake()
    {
        //GameObject mainCam = GameObject.Find("Main Camera");
        //orange = mainCam.GetComponent<OompaLoompa>();

        OompaLoompa[] oranges = GameObject.FindObjectsOfType<OompaLoompa>();
        orange = oranges[0];

        GameObject.FindObjectOfType<Camera>();

    }
    // Use this for initialization
    void Start () {
        Debug.Log("Test : " + orange.GoldenTicket);

        StartCoroutine( CoroutineTest() );
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetKey(KeyCode.UpArrow))
        {
            this.transform.Translate(new Vector3(1 * Time.deltaTime, 0, 0));
        }
	}

    private void FixedUpdate()
    {
        
    }

    public IEnumerator CoroutineTest()
    {
        while (true)
        {
            Debug.Log(Time.time);
            yield return new WaitForSeconds(1.0f);

            Debug.Log("BOOM)");

            yield return new WaitForSeconds(1.0f);

            //GameObject.Destroy(this.gameObject);
        }
    }
}
