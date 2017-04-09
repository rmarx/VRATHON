using UnityEngine;
using System.Collections;
using Valve.VR;

public class Tiling : MonoBehaviour {

    public Transform tile;

	// Use this for initialization

    private float size = 9f;

	void Start () {
	    for (int y = 0; y < 5; y++) {
	        for (int x = 0; x < 5; x++) {
	            Instantiate(tile, new Vector3(x*size, 0, y*size), Quaternion.Euler(new Vector3(-90f, 0, 0)));
	        }
	    }
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}