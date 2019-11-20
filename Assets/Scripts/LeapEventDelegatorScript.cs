using UnityEngine;
using UnityEngine.XR;
using System;
using System.Collections;

public class LeapEventDelegatorScript : MonoBehaviour {

	public GameObject LeapLeftHandAttachments, LeapRightHandAttachments;
	public CreateModeScript createModeScript;
	public EditModeScript editModeScript;
	public ContextSelectionScript contextSelectionScript;
	public HandleScript handleScript;
	public DeleteModeScript deleteModeScript;

	//leap events that other scripts can subscribe to
	/*public delegate void LeapGestureEvent (GameObject caller, GameObject target); 

	public event LeapGestureEvent rpStart;		//Right Pinch Start
	public event LeapGestureEvent rpEnd;		//Right pinch end
	public event LeapGestureEvent rgStart;		//Right grag start
	public event LeapGestureEvent rgEnd;		//Right grab end
	public event LeapGestureEvent riTarget;		//Right index target
	public event LeapGestureEvent rtTarget;		//Right thumb target
	public event LeapGestureEvent rtDown;		//Right thumb down
	public event LeapGestureEvent lgStart;		//Left grab start
	public event LeapGestureEvent lgEnd;		//Left grab end*/

	//The leap based rotation takes the 3D grab point as an argument -  this allows for 3D rotational freedom around the center point.
	//In sreen space which is a plane, this would only allow for rotation around the srcreen's z axis. But we need at least two axes and preferably around x and y axis.
	//In the cases of rotation around atom or bond (only 1 axis), as long as the axis of rotation is not parallel to the sreen's x or y axis, the intersecting plane allows
	//for rotation around that axis. But for the left hand rotation which uses all 3 axes, the mouse version can use mouse input in screen space for 2 axes or 
	//some hack like gizmos or shortcut keys for axis selection. I've chosen to use the screen space for 2 axis rotation. But this will be in world space, not the local space of 
	//the molecule.

	//Leap variables determined by callback functions
	private GameObject tempPinchTarget = null, tempGrabTarget = null;
	private bool tempIndexPointing, tempThumbPointing;


    void Start()
    {
        StartCoroutine(LoadDevice("OpenVR"));
    }

    IEnumerator LoadDevice(string newDevice)
    {
        if (string.Compare(XRSettings.loadedDeviceName, newDevice, true) != 0)
        {
            XRSettings.LoadDeviceByName(newDevice);
            yield return null;
            XRSettings.enabled = true;
        }
    }

    //leap input callback functions
    public void RightSetPinchTarget(GameObject _pinchTarget)
	{
		tempPinchTarget = _pinchTarget;
	}

	public void RightClearPinchTarget()
	{
		tempPinchTarget = null;
	}

	public void RightPinchStart(GameObject _rPinchPoint)
	{
		//if(rpStart != null)
		//	rpStart (_rPinchPoint, tempPinchTarget);
		createModeScript.StartCreate(_rPinchPoint, tempPinchTarget);
	}

	public void RightPinchEnd(GameObject _rPinchPoint)
	{
		//if (rpEnd != null)
		//	rpEnd (_rPinchPoint, tempPinchTarget);
		createModeScript.EndCreate(_rPinchPoint, tempPinchTarget);
	}

	public void RightSetGrabObject(GameObject _rGrabTarget)
	{
		tempGrabTarget = _rGrabTarget;
	}

	public void RightClearGrabTarget()
	{
		tempGrabTarget = null;
	}

	public void RightGrabStart(GameObject _rGrabPoint)
	{
		//if (rgStart != null)
		//	rgStart (_rGrabPoint, tempGrabTarget);
		editModeScript.StartMove(_rGrabPoint, tempGrabTarget);
	}

	public void RightGrabEnd()
	{
		//if (rgEnd != null)
		//	rgEnd (null, null);
		editModeScript.EndMove (null, null);
	}
		
	public void RightSetIndexTarget(GameObject _rIndexTarget)
	{
		//if (riTarget != null && tempIndexPointing)
		//	riTarget (null, _rIndexTarget);
		if (tempIndexPointing)
			contextSelectionScript.DelegateWrapper(null, _rIndexTarget);
	}

	public void RightIndexPointing(bool _rIndexPointing)
	{
		tempIndexPointing = _rIndexPointing;
	}

	public void LeftGrabStart(GameObject _lGrabPoint)
	{
		//if (lgStart != null)
		//	lgStart (_lGrabPoint, null);
		handleScript.StartRotate(_lGrabPoint, null);
	}

	public void LeftGrabEnd()
	{
		//if (lgEnd != null)
		//	lgEnd (null, null);
		handleScript.EndRotate(null, null);
	}

	public void RightSetThumbTarget(GameObject _rThumbTarget)
	{
		//if (rtTarget != null && tempThumbPointing)
		//	rtTarget (null, _rThumbTarget);
		if (tempThumbPointing)
			deleteModeScript.DeleteObject(null, _rThumbTarget);
	}

	public void RightThumbIndexPointing(bool _rThumbPointing)
	{
		tempThumbPointing = _rThumbPointing;
	}

	public void RightThumbsDown(bool _rThumbsDown)
	{
		//if (rtDown != null)
		//	rtDown (null, null);
		contextSelectionScript.Deselection();
	}
}