using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityScript;
using VRTK;
using Valve.VR;


public class EventListener_Vive : MonoBehaviour {

	public CreateModeScript createModeScript;
	public HandleScript handleScript;
	public DeleteModeScript deleteModeScript;
    public EditModeScript editModeScript;
    public ContextSelectionScript contextSelectionScript;
	public VRCanvasScript vrCanvasScript;
	public VRTK_BasePointerRenderer pointerRenderer;
	public GameObject pointerOrigin, atomSpawnPoint, controllerModel, otherController;
	public Material menumodeMaterial, buildmodeMaterial;

	public float creationDistanceFromController = 0.1f;
    public bool grabbing = false;

	public GameObject atomsBonds, grabPoint;       //TODO REMOVE, USE EVENT

	//public bool togglePointerOnHit = false;

	private enum ViveControllerMode
	{
		buildMode = 0,
		menuMode
	};

	private ViveControllerMode selectedMode;

    private void Start()
    {
        GetComponent<VRTK_ControllerEvents>().ButtonTwoPressed += MenuButtonPress;
    }

    // Use this for initialization
    void OnEnable() {
        Invoke("DelayedStart", 1f);
		atomSpawnPoint.transform.position = pointerOrigin.transform.position + (pointerOrigin.transform.rotation * new Vector3(0, 0, creationDistanceFromController));
    }

    private void OnDisable()
    {
        RemoveListeners();
    }

    void DelayedStart()
    {
        selectedMode = ViveControllerMode.buildMode;
        SetControllerTexture();
        AddListeners();
    }

    private void AddListeners ()
	{

        if (selectedMode == ViveControllerMode.buildMode)
		{
			GetComponent<VRTK_ControllerEvents>().TriggerPressed += BuildModeTriggerPress;
			GetComponent<VRTK_ControllerEvents>().TriggerReleased += BuildModeTriggerRelease;
			GetComponent<VRTK_ControllerEvents>().GripPressed += BuildModeGripPress;
			GetComponent<VRTK_ControllerEvents>().GripReleased += BuildModeGripRelease;
			GetComponent<VRTK_ControllerEvents>().TouchpadPressed += BuildModeTouchpadPress;
			GetComponent<VRTK_DestinationMarker>().DestinationMarkerEnter += RayEnteredValidObject;
			GetComponent<VRTK_DestinationMarker>().DestinationMarkerExit += RayExitedValidObject;
		}
		else if (selectedMode == ViveControllerMode.menuMode)
		{
			GetComponent<VRTK_ControllerEvents>().GripPressed += MenuModeGripPress;
		}
	}

	private void RemoveListeners ()
	{
		if (selectedMode == ViveControllerMode.buildMode)
		{
			GetComponent<VRTK_ControllerEvents>().TriggerPressed -= BuildModeTriggerPress;
			GetComponent<VRTK_ControllerEvents>().TriggerReleased -= BuildModeTriggerRelease;
			GetComponent<VRTK_ControllerEvents>().GripPressed -= BuildModeGripPress;
			GetComponent<VRTK_ControllerEvents>().GripReleased -= BuildModeGripRelease;
			GetComponent<VRTK_ControllerEvents>().TouchpadPressed -= BuildModeTouchpadPress;
			GetComponent<VRTK_DestinationMarker>().DestinationMarkerEnter -= RayEnteredValidObject;
			GetComponent<VRTK_DestinationMarker>().DestinationMarkerExit -= RayExitedValidObject;
		}
		else if (selectedMode == ViveControllerMode.menuMode)
		{
			GetComponent<VRTK_ControllerEvents>().GripPressed -= MenuModeGripPress;
		}

	}

	/*--------------------------------------*/

	public void SwitchControllerMode()	//This could probably be in a new script, consider moving if similar methods (that don't really have anything to do with events) exist
	{
		RemoveListeners();

		//Increment selectedMode by one or loop back to start
		int[] arr = (int[])Enum.GetValues(selectedMode.GetType());
		int i = Array.IndexOf(arr, selectedMode) + 1;
		selectedMode = (arr.Length == i) ? (ViveControllerMode)Enum.Parse(typeof(ViveControllerMode), arr[0].ToString()) : (ViveControllerMode)Enum.Parse(typeof(ViveControllerMode), arr[i].ToString());

		SetControllerTexture();

		AddListeners();
	}

	private void SetControllerTexture()
	{
		if (selectedMode == ViveControllerMode.buildMode)
		{
			var trackpadMaterial = controllerModel.transform.Find("trackpad").GetComponent<Renderer>().material;
			if (trackpadMaterial)
			{
				trackpadMaterial.mainTexture = buildmodeMaterial.mainTexture;
				trackpadMaterial.mainTextureScale = new Vector2(1, -1);
			}
		}
		else if (selectedMode == ViveControllerMode.menuMode)
		{
			var trackpadMaterial = controllerModel.transform.Find("trackpad").GetComponent<Renderer>().material;
			if (trackpadMaterial)
			{
				trackpadMaterial.mainTexture = menumodeMaterial.mainTexture;
				trackpadMaterial.mainTextureScale = new Vector2(1, -1);
			}
		}
	}

	private void MenuButtonPress(object s, ControllerInteractionEventArgs e)
	{
		SwitchControllerMode();

		if (selectedMode == ViveControllerMode.menuMode && !otherController.activeInHierarchy)
			vrCanvasScript.SwitchFollowedController();
		else if (selectedMode == ViveControllerMode.menuMode && otherController.activeInHierarchy && otherController.GetComponent<EventListener_Vive>().selectedMode == ViveControllerMode.menuMode)
		{
			otherController.GetComponent<EventListener_Vive>().SwitchControllerMode();
			vrCanvasScript.SwitchFollowedController();
		}
	}

	private void BuildModeTriggerPress(object s, ControllerInteractionEventArgs e)
	{
		RaycastHit pointerDestinationHit = pointerRenderer.GetDestinationHit();

		if (pointerDestinationHit.transform == null) Debug.Log("NO HIT");
		else Debug.Log("Pointer hit " + pointerDestinationHit.transform.GetInstanceID());

		if (pointerDestinationHit.transform != null)
		{
			if (!(pointerDestinationHit.transform.gameObject.layer == LayerMask.NameToLayer("VR_UI")))
				createModeScript.StartCreate(atomSpawnPoint, pointerDestinationHit.transform != null ? pointerDestinationHit.transform.gameObject : null);
		}
		else
			createModeScript.StartCreate(atomSpawnPoint, pointerDestinationHit.transform != null ? pointerDestinationHit.transform.gameObject : null);
	}

	private void BuildModeTriggerRelease (object s, ControllerInteractionEventArgs e)
	{
		RaycastHit pointerDestinationHit = pointerRenderer.GetDestinationHit();

		createModeScript.EndCreate(atomSpawnPoint, pointerDestinationHit.transform != null ? pointerDestinationHit.transform.gameObject : null);
	}

	private void BuildModeGripPress (object s, ControllerInteractionEventArgs e)
	{
        if (!grabbing && !otherController.GetComponent<EventListener_Vive>().grabbing)
        {
            grabbing = true;
            RaycastHit pointerHit = pointerRenderer.GetDestinationHit();

            if (pointerHit.transform != null && pointerHit.transform.gameObject.layer != LayerMask.NameToLayer("VR_UI"))
            {
                grabPoint.transform.position = pointerHit.transform.position;
                editModeScript.StartMove(grabPoint, pointerHit.transform.gameObject);
            }
            else
            {
                handleScript.StartRotate(pointerOrigin, null);
            }
        }
	}

	private void BuildModeGripRelease (object s, ControllerInteractionEventArgs e)
	{
        if (grabbing)
        {
            handleScript.EndRotate(null, null);
            editModeScript.EndMove(null, null);
            grabbing = false;
        }
	}

	private void BuildModeTouchpadPress(object s, ControllerInteractionEventArgs e)
	{
		Vector2 touchpadPosition = SteamVR_Controller.Input((int)e.controllerReference.actual.GetComponent<SteamVR_TrackedObject>().index).GetAxis(EVRButtonId.k_EButton_SteamVR_Touchpad);
		RaycastHit pointerDestinationHit = pointerRenderer.GetDestinationHit();
		Debug.Log(touchpadPosition);

        if (pointerDestinationHit.transform != null)
        {
            if (touchpadPosition.y > 0.4 && -0.4 < touchpadPosition.x && touchpadPosition.x < 0.4)
            {
                if (pointerDestinationHit.transform.gameObject.GetComponent<AtomManagerScript>() != null || pointerDestinationHit.transform.gameObject.GetComponent<BondManagerScript>() != null)
                    deleteModeScript.DeleteObject(null, pointerDestinationHit.transform.gameObject);
            }

            if (touchpadPosition.y < -0.4 && -0.4 < touchpadPosition.x && touchpadPosition.x < 0.4)
            {
                if (pointerDestinationHit.transform != null && pointerDestinationHit.transform.gameObject.layer != LayerMask.NameToLayer("VR_UI"))
                {
                    contextSelectionScript.DelegateWrapper(null, pointerDestinationHit.transform.gameObject);
                }
            }
        }
	}

	private void RayEnteredValidObject (object s, DestinationMarkerEventArgs e)		
	{
		if (e.target.gameObject.GetComponent<AtomManagerScript>() != null)
		{
			createModeScript.HideCreatedAtom(e.target.gameObject);
		}
	}

	private void RayExitedValidObject (object s, DestinationMarkerEventArgs e)
	{
		createModeScript.ShowCreatedAtom();
	}

	private void MenuModeGripPress (object s, ControllerInteractionEventArgs e)
	{
		vrCanvasScript.FreezeOrUnfreezeMenu();
	}
}