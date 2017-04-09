using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRInputController : MonoBehaviour
{
	public GameObject laserBeamPrefab = null;

	public SteamVR_TrackedObject leftControllerGO = null;
	public SteamVR_TrackedObject rightControllerGO = null;

	public GameObject laserLeft = null;
	public GameObject laserRight = null;

	public GravityApplierVR movementScript = null;

	private SteamVR_Controller.Device leftController
	{
		get
		{
			if (leftControllerGO.enabled && leftControllerGO.gameObject.activeSelf)
				return SteamVR_Controller.Input((int)leftControllerGO.index);
			else
				return null;
		}
	}
	private SteamVR_Controller.Device rightController
	{
		get
		{
			if (rightControllerGO.enabled && rightControllerGO.gameObject.activeSelf)
				return SteamVR_Controller.Input((int)rightControllerGO.index);
			else
				return null;
		}
	}

	// Use this for initialization
	void Awake()
	{
		if (leftControllerGO == null)
		{
			Debug.LogError("ERROR ERROR : MANUALLY ASSIGN CONTROLLER, STEAMVR IS VERY BAD LIBRARY!");
			leftControllerGO = GameObject.Find("Controller (left)").GetComponent<SteamVR_TrackedObject>();
		}
		if (rightControllerGO == null)
		{
			Debug.LogError("ERROR ERROR : MANUALLY ASSIGN CONTROLLER, STEAMVR IS VERY BAD LIBRARY!");
			rightControllerGO = GameObject.Find("Controller (right)").GetComponent<SteamVR_TrackedObject>();
		}

		if (laserBeamPrefab == null)
		{
			laserBeamPrefab = GameObject.Find("LaserBeam");
		}

		laserLeft = (GameObject)GameObject.Instantiate(laserBeamPrefab);
		laserRight = (GameObject)GameObject.Instantiate(laserBeamPrefab);

		if (movementScript == null)
			movementScript = GameObject.FindObjectOfType<GravityApplierVR>();
	}

	// Update is called once per frame
	void Update()
	{
		ShowLaser(laserLeft, leftControllerGO.gameObject);
		ShowLaser(laserRight, rightControllerGO.gameObject);
		ProcessController(leftController, leftControllerGO.gameObject, true);
		ProcessController(rightController, rightControllerGO.gameObject, false);
	}

	private Vector3 hitPoint;
	public GameObject hitIndicator = null;

	/*
	void DrawLaser(GameObject laser, GameObject controllerObject, SteamVR_Controller.Device controller)
	{
		//ShowLaser(laser, controllerObject);
		
		RaycastHit hit;
		
		if ( Physics.Raycast(controllerObject.transform.position, controllerObject.transform.forward, out hit, 10000.0f) )
		{
			hitPoint = hit.point;
			ShowLaser(laser, hit);

			GameObject indicator = GameObject.Instantiate(hitIndicator, hit.point, Quaternion.identity);
			indicator.GetComponent<Rigidbody>().AddRelativeForce(transform.forward.normalized * 200, ForceMode.Acceleration);

			//Controller.TriggerHapticPulse(750);
		}
		
	}
	*/

	private void ShowLaser(GameObject laser, GameObject source)
	{
		laser.SetActive(true);
		laser.transform.position = Vector3.Lerp(source.transform.position, source.transform.forward * 1000.0f, .5f);
		laser.transform.LookAt(source.transform.forward * 1000.0f);

		laser.transform.localScale = new Vector3(laser.transform.localScale.x, laser.transform.localScale.y, Vector3.Distance(source.transform.position, source.transform.forward * 1000.0f));
	}

	void ProcessController(SteamVR_Controller.Device controller, GameObject controllerGO, bool left)
	{
		if (controller == null)
		{
			//Debug.LogError("Controller not found " + left);
			return;
		}

		// 1
		if (controller.GetAxis() != Vector2.zero)
		{
			Debug.Log(left + " " + controller.GetAxis());
		}

		// 2
		if (controller.GetHairTriggerDown())
		{
			Debug.LogError(left + " Trigger Press");
			TriggerPull(controllerGO);
		}

		// 3
		if (controller.GetHairTriggerUp())
		{
			Debug.Log(left + " Trigger Release");
		}

		// 4
		if (controller.GetPressDown(SteamVR_Controller.ButtonMask.Grip))
		{
			Debug.Log(left + " Grip Press");
		}

		// 5
		if (controller.GetPressUp(SteamVR_Controller.ButtonMask.Grip))
		{
			Debug.Log(left + " Grip Release");
		}

		if (controller.GetTouch(SteamVR_Controller.ButtonMask.Touchpad))
		{
			Debug.Log("STOP TOUCHING ME!");
		}
		
		if (controller.GetPressDown(SteamVR_Controller.ButtonMask.Touchpad))
		{
			TriggerSwitch(controllerGO);
		}

		if (controller.GetPressDown(SteamVR_Controller.ButtonMask.ApplicationMenu))
		{
			movementScript.Respawn();
		}

		//Debug.Log("Controller " + left + " has something " + controller.index + " // " + controller.transform.pos);
	}

	public void TriggerPull(GameObject source)
	{
		RaycastHit hit;

		if (Physics.Raycast(source.transform.position, source.transform.forward, out hit, 10000.0f))
		{
			movementScript.Pull(hit.point, hit.normal);
		}
	}

	public void TriggerSwitch(GameObject source)
	{
		RaycastHit hit;

		if (Physics.Raycast(source.transform.position, source.transform.forward, out hit, 10000.0f))
		{
			movementScript.SwitchSurface(hit.point, hit.normal);
		}
	}
}
