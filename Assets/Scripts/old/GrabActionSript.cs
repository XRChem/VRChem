using UnityEngine;
using System.Collections;

public class GrabActionSript : MonoBehaviour {

	private Rigidbody grabPointRigidBody;
	private GameObject _target;

	private bool grabbing = false, validTarget = false;

	//delegate void DelegteGrab();
	//DelegteGrab startGrab, endGrab;
	//set it up so that grab functions are assigned to different functions depending whether we are in creation, selection, movement, or measurement mode
	//and depending on which hand is being used

	void Awake()
	{
		grabPointRigidBody = GetComponent<Rigidbody> ();
	}

	//called by proixmity
	public void SetTarget(GameObject target)
	{
		if (_target == null && !grabbing)
		{
			_target = target;
			validTarget = true;
			//print ("valid grab target");
		}
	}

	public void ClearTarget()
	{
		if (_target)
		{
			if (!_target.GetComponent<FixedJoint> ().connectedBody)
			{
				_target = null;
				validTarget = false;
				//print ("no more grab target");
			}
		}
	}

	//called by grab detection (pinch + no extended fingers)
	//move atoms by grabbing them with all fingers
	public void PickUpTarget()
	{
		grabbing = true;
		if (_target)
		{
			_target.transform.position = transform.position;
			_target.GetComponent<FixedJoint> ().connectedBody = grabPointRigidBody;
			//print ("picking up atom");
		}
	}

	public void ReleaseTarget()
	{
		grabbing = false;
		if (_target && _target.activeInHierarchy)
		{
			if (_target.GetComponent<FixedJoint> ().connectedBody == grabPointRigidBody)
			{
				_target.GetComponent<FixedJoint> ().connectedBody = null;
				//print ("releasing atom");
				_target.GetComponent<AtomManagerScript> ().UpdateBonds ();
			}
		}
		_target = null;
	}

	//this continous update of bonds also helps incase the Leap Hand disappears.
	/*void Update()
	{
		if (grabbing)
			_target.GetComponent<AtomManagerScript> ().UpdateBonds ();
	}*/
		
}
