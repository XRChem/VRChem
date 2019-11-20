using UnityEngine;

public class VRCanvasScript : MonoBehaviour {

	public GameObject leapRigCamera, leapParentObject, viveCamera, viveParentObject, viveController_left, viveController_right;
	public HIDScript hidScript;
	public Vector3 vivePosModifier, leapPosModifier;
	public Quaternion viveRotModifier;

	private GameObject followedController;
	private bool menuFrozen = false;


	// Use this for initialization
	void Start () {
		vivePosModifier = new Vector3 (0f, 0.01f, 0.2f);
		viveRotModifier = Quaternion.Euler(90, 0, 0);

		followedController = viveController_left;

		if (leapRigCamera.activeInHierarchy)
		{
			this.GetComponent<Canvas>().worldCamera = leapRigCamera.GetComponent<Camera>();
			hidScript.SelectHIDWrapper((int)HIDScript.HID.LeapMotion);
			
		}
		else if (viveParentObject.activeInHierarchy)
		{
			this.GetComponent<Canvas>().worldCamera = viveCamera.GetComponent<Camera>();
			hidScript.SelectHIDWrapper((int)HIDScript.HID.OpenVR);
		}

	}

	void Update () {		//For moving the VR Canvas with the player or controller
		if (hidScript.selectedHID == HIDScript.HID.LeapMotion) 
		{
			transform.position = leapRigCamera.transform.position + leapParentObject.transform.TransformVector(leapPosModifier);
			transform.rotation = Quaternion.LookRotation(transform.position - leapRigCamera.transform.position, leapParentObject.transform.up);
		}
		else if (hidScript.selectedHID == HIDScript.HID.OpenVR && !menuFrozen)
		{
			//transform.position = transform.position + new Vector3(0.001f, 0f);

			if (followedController.activeInHierarchy)
			{
				transform.position = followedController.transform.position + (followedController.transform.rotation * vivePosModifier);
				transform.rotation = followedController.transform.rotation * viveRotModifier;
			}
		}
	}

	public void SwitchFollowedController()
	{
		if (viveController_right.activeInHierarchy && viveController_left.activeInHierarchy)
			followedController = (followedController == viveController_left) ? viveController_right : viveController_left;
		else if (!viveController_left.activeInHierarchy)
			followedController = viveController_right;
		else if (!viveController_right.activeInHierarchy)
			followedController = viveController_left;
	}

	public void FreezeOrUnfreezeMenu ()
	{
		if (!followedController.activeInHierarchy)
			SwitchFollowedController();

		menuFrozen = menuFrozen ? false : true;
	}
}